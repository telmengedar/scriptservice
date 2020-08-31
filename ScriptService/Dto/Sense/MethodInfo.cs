using ScriptService.Extensions;

namespace ScriptService.Dto.Sense {

    /// <summary>
    /// info about a method
    /// </summary>
    public class MethodInfo : SenseEntry {

        /// <summary>
        /// method parameters
        /// </summary>
        public ParameterInfo[] Parameters { get; set; }

        /// <summary>
        /// type returned by method
        /// </summary>
        public string Returns { get; set; }

        /// <inheritdoc />
        public override string ToString() {
            if(!string.IsNullOrEmpty(Returns))
                return $"{Name}({string.Join<ParameterInfo>(",", Parameters)}) : {Returns.GetPlainTypeName()}";
            return $"{Name}({string.Join<ParameterInfo>(",", Parameters)}) : void";
        }
    }
}