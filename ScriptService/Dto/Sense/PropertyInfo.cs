using ScriptService.Extensions;

namespace ScriptService.Dto.Sense {

    /// <summary>
    /// property description
    /// </summary>
    public class PropertyInfo : SenseEntry {

        /// <summary>
        /// property type
        /// </summary>
        public string Type { get; set; }

        /// <inheritdoc />
        public override string ToString() {
            return $"{Name} : {Type.GetPlainTypeName()}";
        }
    }
}