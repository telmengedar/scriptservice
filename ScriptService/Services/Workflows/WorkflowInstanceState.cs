using System;
using System.Collections.Generic;

namespace ScriptService.Services.Workflows {
    
    /// <summary>
    /// state of a workflow instance
    /// </summary>
    public class WorkflowInstanceState {
        readonly Dictionary<Guid, object> nodedata=new Dictionary<Guid, object>();

        /// <summary>
        /// creates a new <see cref="WorkflowInstanceState"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="variables">workflow state variables</param>
        public WorkflowInstanceState(WorkableLogger logger, StateVariableProvider variables) {
            Logger = logger;
            Variables = variables;
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
        /// logger for workflow execution
        /// </summary>
        public WorkableLogger Logger { get; }

        /// <summary>
        /// current workflow variables
        /// </summary>
        public StateVariableProvider Variables { get; }
    }
}