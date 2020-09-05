using NightlyCode.Scripting;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// transition to another <see cref="InstanceNode"/>
    /// </summary>
    public class InstanceTransition {

        /// <summary>
        /// condition for transition
        /// </summary>
        public IScript Condition { get; set; }

        /// <summary>
        /// target node to which to change
        /// </summary>
        public IInstanceNode Target { get; set; }
    }
}