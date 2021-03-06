﻿using System;
using System.Collections.Generic;
using ScriptService.Dto.Workflows;

namespace ScriptService.Dto {

    /// <summary>
    /// parameters used when executing a script
    /// </summary>
    public class ExecuteWorkflowParameters {

        /// <summary>
        /// id of script to execute
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// name of script to execute
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// workflow revision to execute
        /// </summary>
        public int? Revision { get; set; }

        /// <summary>
        /// complete workflow data for execution
        /// </summary>
        /// <remarks>
        /// used for tests
        /// </remarks>
        public WorkflowStructure Workflow { get; set; }

        /// <summary>
        /// parameters for script execution
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// specifies time to wait for result
        /// </summary>
        /// <remarks>
        /// if script doesn't finish in the specified timespan, taskinformation of the running task is returned
        /// </remarks>
        public TimeSpan? Wait { get; set; }

        /// <summary>
        /// determines whether to profile execution path and performance
        /// </summary>
        public bool? Profile { get; set; }
    }
}