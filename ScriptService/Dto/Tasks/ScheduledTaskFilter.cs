using System;
using NightlyCode.AspNetCore.Services.Data;

namespace ScriptService.Dto.Tasks {

    /// <summary>
    /// filter for scheduled tasks
    /// </summary>
    public class ScheduledTaskFilter : ListFilter {

        /// <summary>
        /// filters for tasks which match the specified target
        /// </summary>
        public DateTime? DueTime { get; set; }
    }
}