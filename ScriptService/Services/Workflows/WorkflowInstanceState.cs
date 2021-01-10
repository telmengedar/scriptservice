using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScriptService.Services.Workflows {
    
    /// <summary>
    /// state of a workflow instance
    /// </summary>
    public class WorkflowInstanceState {
        readonly Func<string, Task<WorkflowInstance>> workflowprovider;
        readonly Dictionary<Guid, object> nodedata=new Dictionary<Guid, object>();
        readonly Dictionary<string, WorkflowInstance> workflowcache=new Dictionary<string, WorkflowInstance>();

        /// <summary>
        /// creates a new <see cref="WorkflowInstanceState"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="variables">workflow state variables</param>
        /// <param name="workflowprovider">provides workflows by name</param>
        public WorkflowInstanceState(WorkableLogger logger, StateVariableProvider variables, Func<string, Task<WorkflowInstance>> workflowprovider, IWorkflowExecutionService workflowexecutor) {
            this.workflowprovider = workflowprovider;
            Logger = logger;
            Variables = variables;
            WorkflowExecutor = workflowexecutor;
        }
        
        /// <summary>
        /// accessor for node instance data
        /// </summary>
        /// <param name="nodeid"></param>
        public object this[Guid nodeid] {
            get {
                nodedata.TryGetValue(nodeid, out object data);
                return data;
            }
            set => nodedata[nodeid] = value;
        }

        /// <summary>
        /// get data stored for a specific node
        /// </summary>
        /// <param name="nodeid">id of node</param>
        /// <typeparam name="T">type of data to get</typeparam>
        /// <returns>node data if any, null otherwise</returns>
        public T GetNodeData<T>(Guid nodeid) {
            return (T) this[nodeid];
        }

        /// <summary>
        /// removes node specific data from state
        /// </summary>
        /// <param name="nodeid">id of node of which to remove data</param>
        public void RemoveNodeData(Guid nodeid) {
            nodedata.Remove(nodeid);
        }

        /// <summary>
        /// get a workflow instance by workflow name
        /// </summary>
        /// <param name="name">name of workflow to get</param>
        /// <returns>workflow instance with the specified name</returns>
        public async Task<WorkflowInstance> GetWorkflow(string name) {
            if (workflowcache.TryGetValue(name, out WorkflowInstance workflow))
                return workflow;

            workflow = await workflowprovider(name);
            workflowcache[name] = workflow;
            
            return workflow;
        }
        
        /// <summary>
        /// logger for workflow execution
        /// </summary>
        public WorkableLogger Logger { get; }

        /// <summary>
        /// current workflow variables
        /// </summary>
        public StateVariableProvider Variables { get; }

        /// <summary>
        /// executor for workflows
        /// </summary>
        public IWorkflowExecutionService WorkflowExecutor { get; }
    }
}