namespace ScriptService.Dto.Sense {

    /// <summary>
    /// entry in a sense table
    /// </summary>
    public class SenseEntry {

        /// <summary>
        /// name of entry
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// documentation for entry
        /// </summary>
        public string Description { get; set; }

        /// <inheritdoc />
        public override string ToString() {
            return Name;
        }
    }
}