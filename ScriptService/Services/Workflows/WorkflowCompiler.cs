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

        /// <summary>
        /// creates a new <see cref="WorkflowCompiler"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="cacheservice">access to object cache</param>
        /// <param name="workflowservice">access to workflow data</param>
        /// <param name="compiler">compiles scripts for conditions and internal workflow code</param>
        public WorkflowCompiler(ILogger<WorkflowCompiler> logger, ICacheService cacheservice, IWorkflowService workflowservice, IScriptCompiler compiler) {
            this.logger = logger;
            this.cacheservice = cacheservice;
            this.workflowservice = workflowservice;
            this.compiler = compiler;
        }
        
        /// <inheritdoc />
        public async Task<WorkflowInstance> BuildWorkflow(WorkflowStructure workflow) {
            logger.LogInformation("Building workflow '{name}'", workflow.Name);
            int startcount = workflow.Nodes.Count(n => n.Type == NodeType.Start);
            if(startcount == 0)
                throw new ArgumentException("Workflow has no start node");
            if(startcount > 1)
                throw new ArgumentException("Workflow has more than one start node");

            StartNode startnode = null;

            List<IInstanceNode> nodes = new List<IInstanceNode>();
            foreach(NodeData node in workflow.Nodes) {
                IInstanceNode nodeinstance = await BuildNode(workflow.Language, node);
                nodes.Add(nodeinstance);
                if(nodeinstance is StartNode startinstance)
                    startnode = startinstance;
            }

            foreach(IndexTransition transition in workflow.Transitions) {
                await BuildTransition(workflow, transition.OriginIndex, transition.TargetIndex, transition, i => nodes[i]);
            }

            return new WorkflowInstance {
                Name = workflow.Name,
                StartNode = startnode,
                Language = workflow.Language
            };
        }

        async Task BuildTransition<T>(WorkflowData workflow, T source, T target, Transition data, Func<T, IInstanceNode> nodegetter) {
            IScript condition = string.IsNullOrEmpty(data.Condition) ? null : await compiler.CompileCodeAsync(data.Condition, data.Language ?? workflow.Language ?? ScriptLanguage.NCScript);
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
                Log = string.IsNullOrEmpty(data.Log) ? null : await compiler.CompileCodeAsync(data.Log, data.Language ?? workflow.Language ?? ScriptLanguage.NCScript)
            });
        }

        async Task<IInstanceNode> BuildNode(ScriptLanguage? language, NodeData node, Guid? nodeid=null) {
            nodeid ??= Guid.NewGuid();
            
            IInstanceNode instance;
            switch(node.Type) {
            case NodeType.Start:
                instance = new StartNode(nodeid.Value, node.Name, node.Parameters.Deserialize<StartParameters>(), compiler);
                break;
            case NodeType.Expression:
                ExecuteExpressionParameters parameters = node.Parameters.Deserialize<ExecuteExpressionParameters>();
                instance = new ExpressionNode(nodeid.Value, node.Name, await compiler.CompileCodeAsync(parameters.Code, parameters.Language));
                break;
            case NodeType.Script:
                instance = new ScriptNode(nodeid.Value, node.Name, node.Parameters.Deserialize<CallWorkableParameters>(), compiler, language);
                break;
            case NodeType.Workflow:
                instance = new WorkflowInstanceNode(nodeid.Value, node.Name, node.Parameters.Deserialize<CallWorkableParameters>(), compiler, language);
                break;
            case NodeType.BinaryOperation:
                BinaryOpParameters binparameters = node.Parameters.Deserialize<BinaryOpParameters>();
                instance = new BinaryNode(nodeid.Value, node.Name, binparameters, compiler);
                break;
            case NodeType.Value:
                ValueParameters valueparameters = node.Parameters.Deserialize<ValueParameters>();
                instance = new ValueNode(nodeid.Value, node.Name, valueparameters.Value, compiler);
                break;
            case NodeType.Suspend:
                instance = new SuspendNode(nodeid.Value, node.Name, node.Parameters.Deserialize<SuspendParameters>());
                break;
            case NodeType.Call:
                instance = new CallNode(nodeid.Value, node.Name, node.Parameters.Deserialize<CallParameters>(), compiler);
                break;
            case NodeType.Iterator:
                instance = new IteratorNode(nodeid.Value, node.Name, node.Parameters.Deserialize<IteratorParameters>(), compiler);
                break;
            case NodeType.Log:
                instance = new LogNode(nodeid.Value, node.Name, compiler, node.Parameters.Deserialize<LogParameters>());
                break;
            default:
                instance = new InstanceNode(nodeid.Value, node.Name);
                break;
            }

            if (!string.IsNullOrEmpty(node.Variable))
                instance = new AssignStateNode(instance, node.Variable, node.VariableOperation, compiler);
            return instance;
        }

        /// <inheritdoc />
        public async Task<WorkflowInstance> BuildWorkflow(WorkflowDetails workflow) {
            WorkflowInstance instance = cacheservice.GetObject<WorkflowInstance, long>(workflow.Id, workflow.Revision);
            if(instance != null)
                return instance;

            logger.LogInformation("Rebuilding workflow '{name}'", workflow.Name);
            int startcount = workflow.Nodes.Count(n => n.Type == NodeType.Start);
            if(startcount == 0)
                throw new ArgumentException("Workflow has no start node");
            if(startcount > 1)
                throw new ArgumentException("Workflow has more than one start node");

            StartNode startnode = null;

            Dictionary<Guid, IInstanceNode> nodes = new Dictionary<Guid, IInstanceNode>();
            foreach(NodeDetails node in workflow.Nodes) {
                IInstanceNode nodeinstance = await BuildNode(workflow.Language, node, node.Id);
                nodes[node.Id] = nodeinstance;

                if(nodeinstance is StartNode startinstance) {
                    startnode = startinstance;
                }
            }

            foreach(TransitionData transition in workflow.Transitions)
                await BuildTransition(workflow, transition.OriginId, transition.TargetId, transition, id => nodes[id]);

            instance = new WorkflowInstance {
                Id = workflow.Id,
                Revision = workflow.Revision,
                Name = workflow.Name,
                Language = workflow.Language,
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