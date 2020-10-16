using System;
using System.Linq;
using ScriptService.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting;
using ScriptService.Dto;
using ScriptService.Dto.Workflows;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services.Cache;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Services.Workflows {

    /// <inheritdoc />
    public class WorkflowCompiler : IWorkflowCompiler {
        readonly ILogger<WorkflowCompiler> logger;
        readonly ICacheService cacheservice;
        readonly IWorkflowService workflowservice;
        readonly IScriptCompiler compiler;
        readonly IWorkflowExecutionService workflowexecutor;

        /// <summary>
        /// creates a new <see cref="WorkflowCompiler"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="cacheservice">access to object cache</param>
        /// <param name="workflowservice">access to workflow data</param>
        /// <param name="compiler">compiles scripts for conditions and internal workflow code</param>
        /// <param name="workflowexecutor">executes workflow instances</param>
        public WorkflowCompiler(ILogger<WorkflowCompiler> logger, ICacheService cacheservice, IWorkflowService workflowservice, IScriptCompiler compiler, IWorkflowExecutionService workflowexecutor) {
            this.logger = logger;
            this.cacheservice = cacheservice;
            this.workflowservice = workflowservice;
            this.compiler = compiler;
            this.workflowexecutor = workflowexecutor;
        }

        async Task<WorkflowInstance> GetWorkflowInstance(string name) {
            WorkflowDetails workflow = await workflowservice.GetWorkflow(name);
            WorkflowInstance instance = await BuildWorkflow(workflow);
            return instance;
        }

        /// <inheritdoc />
        public async Task<WorkflowInstance> BuildWorkflow(WorkflowStructure workflow) {
            logger.LogInformation($"Building workflow '{workflow.Name}'");
            int startcount = workflow.Nodes.Count(n => n.Type == NodeType.Start);
            if(startcount == 0)
                throw new ArgumentException("Workflow has no start node");
            if(startcount > 1)
                throw new ArgumentException("Workflow has more than one start node");

            StartNode startnode = null;

            List<IInstanceNode> nodes = new List<IInstanceNode>();
            foreach(NodeData node in workflow.Nodes) {
                IInstanceNode nodeinstance = await BuildNode(node);
                nodes.Add(nodeinstance);
                if(nodeinstance is StartNode startinstance) {
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

        async Task BuildTransition<T>(T source, T target, Transition data, Func<T, IInstanceNode> nodegetter) {
            IScript condition = string.IsNullOrEmpty(data.Condition) ? null : await compiler.CompileCodeAsync(data.Condition, ScriptLanguage.NCScript);
            List<InstanceTransition> transitions;
            switch(data.Type) {
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
                Condition = condition,
                Log = string.IsNullOrEmpty(data.Log) ? null : await compiler.CompileCodeAsync(data.Log, ScriptLanguage.NCScript)
            });
        }

        async Task<IInstanceNode> BuildNode(NodeData node) {
            IInstanceNode instance;
            switch(node.Type) {
            case NodeType.Start:
                instance = new StartNode(node.Name, node.Parameters.Deserialize<StartParameters>());
                break;
            case NodeType.Expression:
                ExecuteExpressionParameters parameters = node.Parameters.Deserialize<ExecuteExpressionParameters>();
                instance = new ExpressionNode(node.Name, await compiler.CompileCodeAsync(parameters.Code, parameters.Language));
                break;
            case NodeType.Script:
                instance = new ScriptNode(node.Name, node.Parameters.Deserialize<CallWorkableParameters>(), compiler);
                break;
            case NodeType.Workflow:
                instance = new WorkflowInstanceNode(node.Name, node.Parameters.Deserialize<CallWorkableParameters>(), GetWorkflowInstance, workflowexecutor.Execute, compiler);
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
                instance = new CallNode(node.Name, node.Parameters.Deserialize<CallParameters>(), compiler);
                break;
            case NodeType.Iterator:
                instance = new IteratorNode(node.Name, node.Parameters.Deserialize<IteratorParameters>(), compiler);
                break;
            case NodeType.Log:
                instance = new LogNode(node.Name, compiler, node.Parameters.Deserialize<LogParameters>());
                break;
            default:
                instance = new InstanceNode(node.Name);
                break;
            }

            if(!string.IsNullOrEmpty(node.Variable))
                instance = new AssignStateNode(instance, node.Variable);
            return instance;
        }

        /// <inheritdoc />
        public async Task<WorkflowInstance> BuildWorkflow(WorkflowDetails workflow) {
            WorkflowInstance instance = cacheservice.GetObject<WorkflowInstance, long>(workflow.Id, workflow.Revision);
            if(instance != null)
                return instance;

            logger.LogInformation($"Rebuilding workflow '{workflow.Name}'");
            int startcount = workflow.Nodes.Count(n => n.Type == NodeType.Start);
            if(startcount == 0)
                throw new ArgumentException("Workflow has no start node");
            if(startcount > 1)
                throw new ArgumentException("Workflow has more than one start node");

            StartNode startnode = null;

            Dictionary<Guid, IInstanceNode> nodes = new Dictionary<Guid, IInstanceNode>();
            foreach(NodeDetails node in workflow.Nodes) {
                IInstanceNode nodeinstance = await BuildNode(node);
                nodes[node.Id] = nodeinstance;

                if(nodeinstance is StartNode startinstance) {
                    startnode = startinstance;
                }
            }

            foreach(TransitionData transition in workflow.Transitions) {
                await BuildTransition(transition.OriginId, transition.TargetId, transition, id => nodes[id]);
            }

            instance = new WorkflowInstance {
                Id = workflow.Id,
                Revision = workflow.Revision,
                Name = workflow.Name,
                StartNode = startnode
            };
            cacheservice.StoreObject(workflow.Id, workflow.Revision, instance);
            return instance;
        }

        /// <inheritdoc />
        public async Task<WorkflowInstance> BuildWorkflow(long workflowid, int? revision = null) {
            return await BuildWorkflow(await workflowservice.GetWorkflow(workflowid, revision));
        }

        /// <inheritdoc />
        public async Task<WorkflowInstance> BuildWorkflow(string workflowname, int? revision = null) {
            return await BuildWorkflow(await workflowservice.GetWorkflow(workflowname, revision));
        }
    }
}