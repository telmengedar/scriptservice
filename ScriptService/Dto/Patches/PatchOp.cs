namespace ScriptService.Dto.Patches {

    /// <summary>
    /// supported patch operations
    /// </summary>
    public static class PatchOp {

        /// <summary>
        /// replaces a value
        /// </summary>
        public const string Replace = "replace";

        /// <summary>
        /// adds a value to a collection
        /// </summary>
        public const string Add = "add";

        /// <summary>
        /// removes a value from a collection
        /// </summary>
        public const string Remove = "remove";
    }
}