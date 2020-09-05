using System;
using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto.Tasks {

    /// <summary>
    /// task planned to get executed
    /// </summary>
    [AllowPatch]
    public class ScheduledTask : ScheduledTaskData {

        /// <summary>
        /// id of planned task
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// date when task is targeted to get executed next
        /// </summary>
        [Index("target")]
        public DateTime? Target { get; set; }

        /// <summary>
        /// time when task was executed
        /// </summary>
        public DateTime? LastExecution { get; set; }
    }
}