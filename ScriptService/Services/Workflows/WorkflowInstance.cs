﻿using ScriptService.Dto;
using ScriptService.Services.Workflows.Nodes;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// instances workflow
    /// </summary>
    public class WorkflowInstance {

        /// <summary>
        /// workflow id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// workflow revision
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// name of workflow
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// default script language of workflow
        /// </summary>
        public ScriptLanguage? Language { get; set; }
        
        /// <summary>
        /// node where workflow execution starts
        /// </summary>
        public StartNode StartNode { get; set; }
    }
}