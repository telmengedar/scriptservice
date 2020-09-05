using System;

namespace ScriptService.Dto.Tasks {
    /// <summary>
    /// data of scheduled task
    /// </summary>
    public class ScheduledTaskData {

        /// <summary>
        /// name of job
        /// </summary>
        [AllowPatch]
        public string Name { get; set; }

        /// <summary>
        /// workable to execute
        /// </summary>
        [AllowPatch]
        public WorkableType WorkableType { get; set; }

        /// <summary>
        /// name of workable to execute
        /// </summary>
        [AllowPatch]
        public string WorkableName { get; set; }

        /// <summary>
        /// revision of workable to execute (optional)
        /// </summary>
        [AllowPatch]
        public int? WorkableRevision { get; set; }

        /// <summary>
        /// days on which to execute task
        /// </summary>
        [AllowPatch]
        public ScheduledDays Days { get; set; }

        /// <summary>
        /// interval used to repeat execution
        /// </summary>
        [AllowPatch]
        public TimeSpan? Interval { get; set; }
    }
}