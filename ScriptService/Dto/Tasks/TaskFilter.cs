using System;
using NightlyCode.AspNetCore.Services.Data;

namespace ScriptService.Dto.Tasks {

    /// <summary>
    /// list filter for script tasks
    /// </summary>
    public class TaskFilter : ListFilter {

        /// <summary>
        /// status to filter for
        /// </summary>
        public TaskStatus[] Status { get; set; }

        /// <summary>
        /// timewindow start
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// timewindow end
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// id of script to filter for
        /// </summary>
        public long? WorkableId { get; set; }

        /// <summary>
        /// name of script to filter for
        /// </summary>
        public string WorkableName { get; set; }
    }
}