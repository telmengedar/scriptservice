using System;
using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto.Tasks {

    /// <summary>
    /// script task archived in database
    /// </summary>
    [Table("task")]
    public class TaskDb {

        /// <summary>
        /// id of script instance
        /// </summary>
        [PrimaryKey]
        public Guid Id { get; set; }

        /// <summary>
        /// type of workable
        /// </summary>
        public WorkableType Type { get; set; }

        /// <summary>
        /// id of executing script
        /// </summary>
        [Index("script")]
        public long WorkableId { get; set; }

        /// <summary>
        /// executing script revision
        /// </summary>
        public int WorkableRevision { get; set; }

        /// <summary>
        /// name of executing script
        /// </summary>
        [Index("workablename")]
        public string WorkableName { get; set; }

        /// <summary>
        /// script variables passed to script
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// script log
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        /// time when script was started
        /// </summary>
        [Index("timewindow")]
        public DateTime Started { get; set; }

        /// <summary>
        /// time when script finished
        /// </summary>
        [Index("timewindow")]
        public DateTime Finished { get; set; }

        /// <summary>
        /// status of script
        /// </summary>
        [Index("status")]
        public TaskStatus Status { get; set; }

        /// <summary>
        /// script result
        /// </summary>
        public string Result { get; set; }
    }
}