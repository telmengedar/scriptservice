﻿using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto {

    /// <summary>
    /// data of a script
    /// </summary>
    public class ScriptData {

        /// <summary>
        /// name of script
        /// </summary>
        [AllowPatch]
        [Unique]
        public string Name { get; set; }

        /// <summary>
        /// code to execute
        /// </summary>
        [AllowPatch]
        public string Code { get; set; }
    }
}