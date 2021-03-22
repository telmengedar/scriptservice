using ScriptService.Dto;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// state used to continue suspended workflows
    /// </summary>
    public class SuspendState {
        /// <summary>
        /// creates a new <see cref="SuspendState"/>
        /// </summary>
        /// <param name="node">suspended node</param>
        /// <param name="variables">variables of executing workflow</param>
        /// <param name="language">default language of workflow</param>
        /// <param name="profiling">determines whether profiling is enabled</param>
        /// <param name="subflow">suspended sub workflow (optional)</param>
        public SuspendState(IInstanceNode node, StateVariableProvider variables, ScriptLanguage? language, bool profiling, SuspendState subflow=null) {
            Variables = variables;
            Node = node;
            Subflow = subflow;
            Language = language;
            Profiling = profiling;
        }

        /// <summary>
        /// script variables
        /// </summary>
        public StateVariableProvider Variables { get; }
        
        /// <summary>
        /// node at which workflow was suspended
        /// </summary>
        public IInstanceNode Node { get; }

        /// <summary>
        /// suspended state of sub workflow
        /// </summary>
        public SuspendState Subflow { get; }

        /// <summary>
        /// default language used in workflow
        /// </summary>
        public ScriptLanguage? Language { get; }

        /// <summary>
        /// determines whether profiling is enabled
        /// </summary>
        public bool Profiling { get; }
        
        /// <inheritdoc />
        public override string ToString() {
            if (Subflow != null)
                return $"{Node.NodeName}/{Subflow}";
            return Node.NodeName;
        }
    }
}