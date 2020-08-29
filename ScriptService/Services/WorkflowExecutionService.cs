using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Providers;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;
using ScriptService.Services.Cache;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;
using ScriptService.Services.Workflows.Commands;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Services {

    /// <inheritdoc />
    public class WorkflowExecutionService : IWorkflowExecutionService {
        readonly ILogger<WorkflowExecutionService> logger;
        readonly IWorkflowService workflowservice;
        readonly ITaskService taskservice;
        readonly IScriptCompiler compiler;
        readonly ICacheService cache;
        readonly IScriptService scriptservice;
        readonly IImportProvider importprovider;

        /// <summary>
        /// creates a new <see cref="WorkflowExecutionService"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="workflowservice">access to workflow data</param>
        /// <param name="taskservice">access to task information</param>
        /// <param name="compiler">access to script compiling</param>
        /// <param name="cache">access to object cache</param>
        /// <param name="scriptservice">access to script data</param>
        /// <param name="importprovider">access to host imports</param>
        public WorkflowExecutionService(ILogger<WorkflowExecutionService> logger, IWorkflowService workflowservice, ITaskService taskservice, IScriptCompiler compiler, ICacheService cache, IScriptService scriptservice, IImportProvider importprovider) {
            this.logger = logger;
            this.workflowservice = workflowservice;
            this.taskservice = taskservice;
            this.compiler = compiler;
            this.cache = cache;
            this.scriptservice = scriptservice;
            this.importprovider = importprovider;
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(long id, IDictionary<string, object> variables = null, TimeSpan? wait = null) {
            WorkflowDetails workflow = await workflowservice.GetWorkflow(id);
            WorkflowInstance instance = await BuildWorkflow(workflow, variables);
            return await Execute(instance, wait);
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(string name, IDictionary<string, object> variables = null, TimeSpan? wait = null) {
            WorkflowDetails workflow = await workflowservice.GetWorkflow(name);
            WorkflowInstance instance = await BuildWorkflow(workflow, variables);
            return await Execute(instance, wait);
        }

        async Task<WorkflowInstance> GetWorkflowInstance(string name, IDictionary<string, object> variables = null) {
            WorkflowDetails workflow = await workflowservice.GetWorkflow(name);
            WorkflowInstance instance = await BuildWorkflow(workflow, variables);
            return instance;
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(WorkflowStructure workflow, IDictionary<string, object> variables = null, TimeSpan? wait = null) {
            WorkflowInstance instance = await BuildWorkflow(workflow, variables);
            return await Execute(instance, wait);
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Continue(Guid taskid, IDictionary<string, object> variables=null, TimeSpan? wait = null) {
            WorkableTask task = await taskservice.GetTask(taskid);
            if (task.Status != TaskStatus.Suspended)
                throw new ArgumentException($"Task '{taskid}' is not suspended.");
            if (task.SuspensionState == null)
                throw new InvalidOperationException($"Task '{taskid}' has no suspension state linked to it.");

            WorkableLogger tasklogger = new WorkableLogger(logger, task);
            tasklogger.Info("Resuming execution of workflow", string.Join("\n", variables?.Select(p => $"{p.Key}={p.Value}") ?? new string[0]));

            try {
                task.Status = TaskStatus.Running;
                task.Task = Task.Run(() => ContinueState(task.SuspensionState, tasklogger, variables, task.Token.Token)).ContinueWith(t => HandleTaskResult(t, task, tasklogger));
            }
            catch(Exception e) {
                tasklogger.Error("Failed to execute workflow", e);
                task.Finished = DateTime.Now;
                task.Status = TaskStatus.Failure;
                await taskservice.FinishTask(task.Id);
            }

            if(wait.HasValue && !task.Task.IsCompleted)
                await Task.WhenAny(task.Task, Task.Delay(wait.Value));

            return task;
        }

        async Task<object> ContinueState(SuspendState state, WorkableLogger tasklogger, IDictionary<string, object> variables, CancellationToken token) {
            object lastresult = null;
            if (state.Subflow != null)
                lastresult = await ContinueState(state.Subflow, tasklogger, variables, token);
            else {
                if(variables!=null)
                    foreach (KeyValuePair<string, object> entry in variables)
                        state.State[entry.Key] = entry.Value.DetermineValue(state.State);
            }

            return await Execute(state.Variables, tasklogger, token, state.State, await EvaluateTransitions(state.Node, tasklogger, state.State, state.Node.Transitions, token), lastresult);
        }

        async Task<IInstanceNode> BuildNode(NodeData node) {
            IInstanceNode instance;
            switch (node.Type) {
            case NodeType.Start:
                instance = new StartNode(node.Name, node.Parameters.Deserialize<StartParameters>());
                break;
            case NodeType.Expression:
                ExecuteExpressionParameters parameters = node.Parameters.Deserialize<ExecuteExpressionParameters>();
                instance = new ExpressionNode(node.Name, await compiler.CompileCode(parameters.Code));
                break;
            case NodeType.Script:
                CallWorkableParameters scriptparameters = node.Parameters.Deserialize<CallWorkableParameters>();
                Script script = await scriptservice.GetScript(scriptparameters.Name);
                instance = new ScriptNode(node.Name, await compiler.CompileCode(script.Id, script.Revision, script.Code), scriptparameters.Arguments.BuildArguments());
                break;
            case NodeType.Workflow:
                instance = new WorkflowInstanceNode(node.Name, node.Parameters.Deserialize<CallWorkableParameters>(), GetWorkflowInstance, Execute);
                break;
            case NodeType.BinaryOperation:
                BinaryOpParameters binparameters = node.Parameters.Deserialize<BinaryOpParameters>();
                instance = new BinaryNode(node.Name, binparameters, compiler);
                break;
            case NodeType.Value:
                ValueParameters valueparameters = node.Parameters.Deserialize<ValueParameters>();
                instance = new ValueNode(node.Name, valueparameters.Value, compiler);
                break;
            case NodeType.Suspend:
                instance = new SuspendNode(node.Name, node.Parameters.Deserialize<SuspendParameters>());
                break;
            case NodeType.Call:
                instance=new CallNode(node.Name, node.Parameters.Deserialize<CallParameters>(), compiler);
                break;
            case NodeType.Iterator:
                instance = new IteratorNode(node.Name, node.Parameters.Deserialize<IteratorParameters>(), compiler);
                break;
            default:
                instance = new InstanceNode(node.Name);
                break;
            }

            if (!string.IsNullOrEmpty(node.Variable))
                instance = new AssignStateNode(instance, node.Variable);
            return instance;
        }

        async Task BuildTransition<T>(T source, T target, Transition data, Func<T, IInstanceNode> nodegetter) {
            IScript condition = string.IsNullOrEmpty(data.Condition) ? null : await compiler.CompileCode(data.Condition);
            List<InstanceTransition> transitions;
            switch (data.Type) {
            case TransitionType.Standard:
                transitions = nodegetter(source).Transitions;
                break;
            case TransitionType.Error:
                transitions = nodegetter(source).ErrorTransitions;
                break;
            case TransitionType.Loop:
                transitions = nodegetter(source).LoopTransitions;
                break;
            default:
                throw new ArgumentException($"Invalid type '{data.Type}'");
            }

            transitions.Add(new InstanceTransition {
                Target = nodegetter(target),
                Condition = condition
            });
        }

        async Task<WorkflowInstance> BuildWorkflow(WorkflowStructure workflow, IDictionary<string, object> variables) {
            logger.LogInformation($"Building workflow '{workflow.Name}'");
            int startcount = workflow.Nodes.Count(n => n.Type == NodeType.Start);
            if(startcount == 0)
                throw new ArgumentException("Workflow has no start node");
            if(startcount > 1)
                throw new ArgumentException("Workflow has more than one start node");

            StartNode startnode = null;

            List<IInstanceNode> nodes=new List<IInstanceNode>();
            foreach(NodeData node in workflow.Nodes) {
                IInstanceNode nodeinstance = await BuildNode(node);
                nodes.Add(nodeinstance);
                if (nodeinstance is StartNode startinstance) {
                    startinstance.Arguments = variables;
                    startnode = startinstance;
                }
            }

            foreach(IndexTransition transition in workflow.Transitions) {
                await BuildTransition(transition.OriginIndex, transition.TargetIndex, transition, i => nodes[i]);
            }

            return new WorkflowInstance {
                Name = workflow.Name,
                StartNode = startnode
            };
        }

        async Task<WorkflowInstance> BuildWorkflow(WorkflowDetails workflow, IDictionary<string, object> variables) {
            WorkflowInstance instance = cache.GetObject<WorkflowInstance, long>(workflow.Id, workflow.Revision);
            if (instance != null)
                return instance;

            logger.LogInformation($"Rebuilding workflow '{workflow.Name}'");
            int startcount = workflow.Nodes.Count(n => n.Type == NodeType.Start);
            if (startcount == 0)
                throw new ArgumentException("Workflow has no start node");
            if (startcount > 1)
                throw new ArgumentException("Workflow has more than one start node");

            StartNode startnode = null;

            Dictionary<Guid,IInstanceNode> nodes=new Dictionary<Guid, IInstanceNode>();
            foreach (NodeDetails node in workflow.Nodes) {
                IInstanceNode nodeinstance = await BuildNode(node);
                nodes[node.Id] = nodeinstance;

                if (nodeinstance is StartNode startinstance) {
                    startinstance.Arguments = variables;
                    startnode = startinstance;
                }
            }

            foreach (TransitionData transition in workflow.Transitions) {
                await BuildTransition(transition.OriginId, transition.TargetId, transition, id => nodes[id]);
            }

            instance = new WorkflowInstance {
                Id = workflow.Id,
                Revision = workflow.Revision,
                Name = workflow.Name,
                StartNode = startnode
            };
            cache.StoreObject(workflow.Id, workflow.Revision, instance);
            return instance;
        }

        void HandleTaskResult(Task<object> t, WorkableTask task, WorkableLogger tasklogger) {
            if(t.IsCanceled) {
                tasklogger.Warning("Workflow execution was aborted");
                task.Status = TaskStatus.Canceled;
            }
            else if(t.IsFaulted) {
                tasklogger.Error("Workflow failed to execute", t.Exception?.InnerException ?? t.Exception);
                task.Status = TaskStatus.Failure;
            }
            else {
                if(t.Result is SuspendState state) {
                    tasklogger.Info($"Workflow was suspended at '{state}'");
                    task.SuspensionState = state;
                    task.Status = TaskStatus.Suspended;

                    // don't finish task when it gets suspended
                    // else it would get serialized and suspension state is lost
                    // TODO: this could get refactored to allow for state serialization
                    return;
                }

                task.Result = t.Result;
                tasklogger.Info($"Workflow executed successfully with result '{task.Result}'");
                task.Status = TaskStatus.Success;
            }

            task.Finished = DateTime.Now;
            taskservice.FinishTask(task.Id).GetAwaiter().GetResult();
        }

        Dictionary<string, object> ProcessImports(WorkableLogger tasklogger, ImportDeclaration[] imports) {
            Dictionary<string, object> variables = new Dictionary<string, object> {
                ["log"] = tasklogger
            };

            if (imports != null) {
                foreach (ImportDeclaration import in imports)
                    variables[import.Variable] = importprovider.Import(new object[] {import.Type, import.Name});
            }

            return variables;
        }

        async Task<WorkableTask> Execute(WorkflowInstance workflow, TimeSpan? wait = null) {
            WorkableTask task = taskservice.CreateTask(WorkableType.Workflow, workflow.Id, workflow.Revision, workflow.Name, workflow.StartNode.Arguments);
            WorkableLogger tasklogger = new WorkableLogger(logger, task);
            try {
                
                task.Task = Task.Run(() => Execute(workflow, tasklogger, task.Token.Token)).ContinueWith(t => HandleTaskResult(t, task, tasklogger));
            }
            catch (Exception e) {
                tasklogger.Error("Failed to execute workflow", e);
                task.Finished = DateTime.Now;
                task.Status = TaskStatus.Failure;
                await taskservice.FinishTask(task.Id);
            }

            if(wait.HasValue && !task.Task.IsCompleted)
                await Task.WhenAny(task.Task, Task.Delay(wait.Value));

            return task;
        }

        Task<object> Execute(WorkflowInstance workflow, WorkableLogger tasklogger, CancellationToken token) {
            IVariableProvider variables = new VariableProvider(ProcessImports(tasklogger, workflow.StartNode.Parameters?.Imports));
            return Execute(variables, tasklogger, token, new Dictionary<string, object>(), workflow.StartNode);
        }

        Task<IInstanceNode> EvaluateTransitions(IInstanceNode current, WorkableLogger tasklogger, IDictionary<string, object> state, List<InstanceTransition> transitions, CancellationToken token) {
            return EvaluateTransitions(current, tasklogger, new VariableProvider(state), transitions, token);
        }

        async Task<IInstanceNode> EvaluateTransitions(IInstanceNode current, WorkableLogger tasklogger, IVariableProvider variableprovider, List<InstanceTransition> transitions, CancellationToken token) {
            foreach (InstanceTransition conditionaltransition in transitions.Where(c=>c.Condition!=null)) {
                if (await conditionaltransition.Condition.ExecuteAsync<bool>(variableprovider, token)) {
                    tasklogger.Info($"Transition '{current.NodeName}' -> '{conditionaltransition.Target.NodeName}'");
                    return conditionaltransition.Target;
                }
            }

            InstanceTransition[] defaulttransitions = transitions.Where(t => t.Condition == null).ToArray();
            if (defaulttransitions.Length > 0) {
                if (defaulttransitions.Length > 1)
                    tasklogger.Warning($"More than one default transition defined for '{current.NodeName}'. Behavior of Workflow is undefined since the order of transitions is not guaranteed to be static.");
                tasklogger.Info($"Transition '{current.NodeName}' -> '{defaulttransitions.First().Target.NodeName}'");
                return defaulttransitions.First().Target;
            }

            return null;
        }

        async Task<object> Execute(IVariableProvider variables, WorkableLogger tasklogger, CancellationToken token, IDictionary<string, object> state, IInstanceNode current, object lastresult=null) {
            while (current != null) {
                try {
                    lastresult = await current.Execute(tasklogger, variables, state, token);

                    if (token.IsCancellationRequested)
                        return null;

                    // used for workflow suspension
                    if (lastresult is SuspendState)
                        return lastresult;
                }
                catch (Exception e) {
                    IInstanceNode next = await EvaluateTransitions(current, tasklogger, new VariableProvider(new VariableProvider(new Variable("error", e)), state), current.ErrorTransitions, token);
                    if (next == null)
                        throw;

                    tasklogger.Warning($"Error while executing node '{current.NodeName}'", e.Message);
                    current = next;
                    continue;
                }

                try {
                    if (lastresult is LoopCommand) {
                        current = await EvaluateTransitions(current, tasklogger, new VariableProvider(state), current.LoopTransitions, token) ?? current;
                    }
                    else current = await EvaluateTransitions(current, tasklogger, new VariableProvider(state), current.Transitions, token);
                }
                catch (Exception e) {
                    IInstanceNode next = await EvaluateTransitions(current, tasklogger, new VariableProvider(new VariableProvider(new Variable("error", e)), state), current.ErrorTransitions, token);
                    if(next == null)
                        throw;

                    tasklogger.Warning($"Error while evaluating transitions of node '{current.NodeName}'", e.Message);
                    current = next;
                }
            }

            return lastresult;
        }
    }
}