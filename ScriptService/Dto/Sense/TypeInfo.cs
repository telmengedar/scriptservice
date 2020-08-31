using ScriptService.Extensions;

namespace ScriptService.Dto.Sense {

    /// <summary>
    /// info about a type
    /// </summary>
    public class TypeInfo : SenseEntry {

        /// <summary>
        /// type of element if this type is a collection
        /// </summary>
        public string ElementType { get; set; }

        /// <summary>
        /// methods available in type
        /// </summary>
        public MethodInfo[] Methods { get; set; }

        /// <summary>
        /// properties available in type
        /// </summary>
        public PropertyInfo[] Properties { get; set; }

        /// <summary>
        /// indexer methods
        /// </summary>
        public MethodInfo[] Indexer { get; set; }

        /// <inheritdoc />
        public override string ToString() {
            return Name.GetPlainTypeName();
        }
    }
}