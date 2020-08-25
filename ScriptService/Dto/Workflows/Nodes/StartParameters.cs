﻿
namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters for a start node
    /// </summary>
    public class StartParameters {

        /// <summary>
        /// types which are imported by the workflow
        /// </summary>
        public ImportDeclaration[] Imports { get; set; }
    }
}