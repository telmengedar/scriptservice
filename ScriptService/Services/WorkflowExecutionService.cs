using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Providers;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Extensions;
using ScriptService.Services.Workflows;
using ScriptService.Services.Workflows.Commands;
using ScriptService.Services.Workflows.Nodes;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Services {

    /// <inheritdoc />
    public class WorkflowExecutionService : IWorkflowExecutionService {
        readonly ILogger<WorkflowExecutionService> logger;
        readonly ITaskService taskservice;
        readonly IImportProvider importprovider;

        /// <summary>
        /// creates a new <see cref="WorkflowExecutionService"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="taskservice">access to task information</param>
        /// <param name="importprovider">access to host imports</param>
        public WorkflowExecutionService(ILogger<WorkflowExecutionService> logger, ITaskService taskservice, IMethodProviderService importprovider) { 
            this.logger = logger;
            this.taskservice = taskservice;
            this.importprovider = importprovider;
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

            InstanceTransition transition = await EvaluateTransitions(state.Node, tasklogger, state.State, state.Node.Transitions, token);
            if (transition == null) {
                tasklogger.Warning("Suspend node has no transition defined for current state. Workflow ends by default.");
                return lastresult;
            }

            return await Execute(state.Variables, tasklogger, token, state.State, transition.Target, lastresult);
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

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(WorkflowInstance workflow, IDictionary<string, object> arguments, TimeSpan? wait = null) {
            WorkableTask task = taskservice.CreateTask(WorkableType.Workflow, workflow.Id, workflow.Revision, workflow.Name, arguments);
            WorkableLogger tasklogger = new WorkableLogger(logger, task);
            try {
                task.Task = Task.Run(() => Execute(workflow, tasklogger, arguments, task.Token.Token)).ContinueWith(t => HandleTaskResult(t, task, tasklogger));
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

        /// <inheritdoc />
        public Task<object> Execute(WorkflowInstance workflow, WorkableLogger tasklogger, IDictionary<string, object> arguments, CancellationToken token) {
            IVariableProvider variables = new StateVariableProvider(ProcessImports(tasklogger, workflow.StartNode.Parameters?.Imports));
            arguments = arguments.Clone();
            return Execute(variables, tasklogger, token, arguments, workflow.StartNode);
        }

        Task<InstanceTransition> EvaluateTransitions(IInstanceNode current, WorkableLogger tasklogger, IDictionary<string, object> state, List<InstanceTransition> transitions, CancellationToken token) {
            return EvaluateTransitions(current, tasklogger, new VariableProvider(state), transitions, token);
        }

        async Task<InstanceTransition> EvaluateTransitions(IInstanceNode current, WorkableLogger tasklogger, IVariableProvider variableprovider, List<InstanceTransition> transitions, CancellationToken token) {
            InstanceTransition transition = null;
            foreach (InstanceTransition conditionaltransition in transitions.Where(c=>c.Condition!=null)) {
                if (await conditionaltransition.Condition.ExecuteAsync<bool>(variableprovider, token)) {
                    transition= conditionaltransition;
                    break;
                }
            }

            if (transition == null) {
                InstanceTransition[] defaulttransitions = transitions.Where(t => t.Condition == null).ToArray();
                if (defaulttransitions.Length > 0) {
                    if (defaulttransitions.Length > 1)
                        tasklogger.Warning($"More than one default transition defined for '{current.NodeName}'. Behavior of Workflow is undefined since the order of transitions is not guaranteed to be static.");
                    transition = defaulttransitions.First();
                }
            }

            if (transition?.Log != null) {
                try {
                    tasklogger.Info(await transition.Log.ExecuteAsync<string>(variableprovider, token));
                }
                catch (Exception e) {
                    tasklogger.Error($"Error executing transition log of '{current.NodeName}'->'{transition.Target?.NodeName}'", e);
                    throw;
                }
            }
            return transition;
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
                    tasklogger.Warning($"Error while executing node '{current.NodeName}'", e.Message);
                    
                    InstanceTransition next = await EvaluateTransitions(current, tasklogger, new StateVariableProvider(new VariableProvider(variables, new Variable("error", e)), state), current.ErrorTransitions, token);
                    if (next == null)
                        throw;
                    
                    current = next.Target;
                    continue;
                }

                try {
                    if (lastresult is LoopCommand) {
                        InstanceTransition transition = await EvaluateTransitions(current, tasklogger, new StateVariableProvider(variables, state), current.LoopTransitions, token);
                        current =  transition?.Target ?? current;
                    }
                    else {
                        InstanceTransition transition = await EvaluateTransitions(current, tasklogger, new StateVariableProvider(variables, state), current.Transitions, token);
                        current = transition?.Target;
                    }
                }
                catch (Exception e) {
                    tasklogger.Warning($"Error while evaluating transitions of node '{current?.NodeName}'", e.Message);
                    
                    InstanceTransition next = await EvaluateTransitions(current, tasklogger, new StateVariableProvider(new VariableProvider(variables, new Variable("error", e)), state), current.ErrorTransitions, token);
                    if(next == null)
                        throw;
                    
                    current = next.Target;
                }
            }

            return lastresult;
        }
    }
}