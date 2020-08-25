namespace ScriptService.Dto.Patches {
    
    /// <summary>
    /// provides helper methods to create patches
    /// </summary>
    public static class Patch {

        /// <summary>
        /// creates a replace patch (set a value)
        /// </summary>
        /// <param name="property">property to set</param>
        /// <param name="value">value to set</param>
        /// <returns>patch operation to send to patch endpoints</returns>
        public static PatchOperation Replace(string property, object value) {
            return new PatchOperation {
                Op = "replace",
                Path = $"/{property.ToLower()}",
                Value = value
            };
        }
        
        /// <summary>
        /// adds items to a collection
        /// </summary>
        /// <param name="property">path to collection</param>
        /// <param name="value">item or collection to add</param>
        /// <returns>patch operation to send to patch endpoints</returns>
        public static PatchOperation Add(string property, object value) {
            return new PatchOperation {
                Op = "add",
                Path = $"/{property.ToLower()}",
                Value = value
            };
        }

        /// <summary>
        /// removes items from a collection
        /// </summary>
        /// <param name="property">path to collection</param>
        /// <param name="value">item or collection to add</param>
        /// <returns>patch operation to send to patch endpoints</returns>
        public static PatchOperation Remove(string property, object value) {
            return new PatchOperation {
                Op = "remove",
                Path = $"/{property.ToLower()}",
                Value = value
            };
        }
    }
}