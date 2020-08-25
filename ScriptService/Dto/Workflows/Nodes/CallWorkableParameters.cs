using System.Collections.Generic;

namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for a node of type <see cref="NodeType.Script"/>
    /// </summary>
    public class CallWorkableParameters {

        /// <summary>
        /// name of workable to call
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// parameters for script to call
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }
    }
}