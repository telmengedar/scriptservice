using NightlyCode.Scripting;

namespace ScriptService.Dto.Scripts {

    /// <summary>
    /// compiled script with meta data
    /// </summary>
    public class CompiledScript {

        /// <summary>
        /// id of script
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// script revision
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// name of script
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// compiled script instance
        /// </summary>
        public IScript Instance { get; set; }
    }
}