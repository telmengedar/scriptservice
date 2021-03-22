using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ScriptService.Services.Workflows;

namespace ScriptService.Dto.Tasks {

    /// <summary>
    /// currently executing script
    /// </summary>
    public class WorkableTask {

        /// <summary>
        /// creates a new <see cref="WorkableTask"/>
        /// </summary>
        public WorkableTask() {
        }

        /// <summary>
        /// creates a new <see cref="WorkableTask"/>
        /// </summary>
        /// <param name="other">script task copy values from</param>
        public WorkableTask(WorkableTask other) {
            Id = other.Id;
            Type = other.Type;
            WorkableId = other.WorkableId;
            WorkableRevision = other.WorkableRevision;
            WorkableName = other.WorkableName;
            Parameters = other.Parameters;
            Log = other.Log;
            Performance = other.Performance;
            Started = other.Started;
            Finished = other.Finished;
            if (Finished.HasValue)
                Runtime = Finished.Value - Started;
            else Runtime = DateTime.Now - Started;
            Status = other.Status;
            Result = other.Result;
        }

        /// <summary>
        /// id of task
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// type of workable
        /// </summary>
        public WorkableType Type { get; set; }

        /// <summary>
        /// id of executing script
        /// </summary>
        public long WorkableId { get; set; }

        /// <summary>
        /// executing script revision
        /// </summary>
        public int WorkableRevision { get; set; }

        /// <summary>
        /// name of executing script
        /// </summary>
        public string WorkableName { get; set; }

        /// <summary>
        /// script variables passed to script
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// script log
        /// </summary>
        public List<string> Log { get; set; }

        /// <summary>
        /// performance profiling log
        /// </summary>
        public List<ProfilingEntry> Performance { get; set; }
        
        /// <summary>
        /// time when script was started
        /// </summary>
        public DateTime Started { get; set; }

        /// <summary>
        /// time when script finished
        /// </summary>
        public DateTime? Finished { get; set; }

        /// <summary>
        /// time script ran
        /// </summary>
        public TimeSpan Runtime { get; set; }

        /// <summary>
        /// status of script
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// script result
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// internal task used to run the script
        /// </summary>
        [IgnoreDataMember]
        public Task Task { get; set; }

        /// <summary>
        /// token used to cancel task
        /// </summary>
        [IgnoreDataMember]
        public CancellationTokenSource Token { get; set; }

        /// <summary>
        /// state used to continue workflow
        /// </summary>
        [IgnoreDataMember]
        public SuspendState SuspensionState { get; set; }
    }
}