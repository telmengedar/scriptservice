using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto {

    /// <summary>
    /// script for execution
    /// </summary>
    [AllowPatch]
    public class Script : ScriptData {

        /// <summary>
        /// script id
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// revision number
        /// </summary>
        public int Revision { get; set; }
    }
}