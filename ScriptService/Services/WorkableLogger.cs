using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using LogLevel = ScriptService.Dto.LogLevel;

namespace ScriptService.Services {

    /// <summary>
    /// provides logging methods which route to the service logger and the linked task object
    /// </summary>
    public class WorkableLogger {
        readonly ILogger logger;
        readonly WorkableTask instance;
        readonly List<ProfilingEntry> performance = new List<ProfilingEntry>();

        /// <summary>
        /// creates a new <see cref="WorkableLogger"/>
        /// </summary>
        /// <param name="logger">logger to send logs to</param>
        /// <param name="instance">script instance to add logs to</param>
        public WorkableLogger(ILogger logger, WorkableTask instance) {
            this.logger = logger;
            this.instance = instance;
        }

        /// <summary>
        /// adds a performance entry
        /// </summary>
        /// <param name="workflow">executed workflow</param>
        /// <param name="nodeid">id of node</param>
        /// <param name="nodename">node name</param>
        /// <param name="time">time it took to execute node</param>
        public void Performance(WorkflowIdentifier workflow, Guid nodeid, string nodename, TimeSpan time) {
            performance.Add(new ProfilingEntry {
                Workflow = workflow,
                Node = new NodeIdentifier(nodeid, nodename),
                Time = time
            });
        }

        /// <summary>
        /// writes performance entries to the log
        /// </summary>
        public void LogPerformance() {
            foreach (IGrouping<WorkflowIdentifier, ProfilingEntry> workflowperformancegroup in performance.GroupBy(e => e.Workflow)) {
                StringBuilder details = new StringBuilder();
                foreach (IGrouping<NodeIdentifier, ProfilingEntry> nodeperformance in workflowperformancegroup.GroupBy(e => e.Node))
                    details.AppendLine($"{nodeperformance.Key.Name}: {nodeperformance.Count()} calls took {TimeSpan.FromSeconds(nodeperformance.Sum(p => p.Time.TotalSeconds))}");
                Info($"Workflow {workflowperformancegroup.Key.Name}: {workflowperformancegroup.Count()} calls took {TimeSpan.FromSeconds(workflowperformancegroup.Sum(p => p.Time.TotalSeconds))}", details.ToString());
            }
        }
        
        /// <summary>
        /// logs a message
        /// </summary>
        /// <param name="severity">severity of message</param>
        /// <param name="message">message text to log</param>
        /// <param name="details">message details</param>
        public void Log(LogLevel severity, string message, string details = null) {
            switch (severity) {
            case LogLevel.Error:
                Error(message, details);
                break;
            case LogLevel.Warning:
                Warning(message, details);
                break;
            default:
                Info(message, details);
                break;
            }
        }

        /// <summary>
        /// logs an info
        /// </summary>
        /// <param name="message">info message</param>
        /// <param name="details">message details (optional)</param>
        public void Info(string message, string details = null) {
            if(details != null)
                logger.LogInformation("{name}#{revision}: {message}\n{details}", instance.WorkableName, instance.WorkableRevision, message, details);
            else
                logger.LogInformation("{name}#{revision}: {message}", instance.WorkableName, instance.WorkableRevision, message);

            instance.Log.Add($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} INF: {message}");
            if(details != null)
                instance.Log.Add(details);
        }

        /// <summary>
        /// logs a warning
        /// </summary>
        /// <param name="message">warning message</param>
        /// <param name="details">message details (optional)</param>
        public void Warning(string message, string details = null) {
            if(details != null)
                logger.LogWarning("{name}#{revision}: {message}\n{details}", instance.WorkableName, instance.WorkableRevision, message, details);
            else
                logger.LogWarning("{name}#{revision}: {message}", instance.WorkableName, instance.WorkableRevision, message);

            instance.Log.Add($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} WRN: {message}");
            if(details != null)
                instance.Log.Add(details);
        }

        /// <summary>
        /// logs an error
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="details">message details (optional)</param>
        public void Error(string message, string details = null) {
            if(details != null)
                logger.LogError("{name}#{revision}: {message}\n{details}", instance.WorkableName, instance.WorkableRevision, message, details);
            else
                logger.LogError("{name}#{revision}: {message}", instance.WorkableName, instance.WorkableRevision, message);

            instance.Log.Add($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} ERR: {message}");
            if(details != null)
                instance.Log.Add(details);
        }

        /// <summary>
        /// logs an error
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="details">message details (optional)</param>
        public void Error(string message, Exception details = null) {
            if(details != null)
                logger.LogError(details, "{name}#{revision}: {message}", instance.WorkableName, instance.WorkableRevision, message);
            else
                logger.LogError("{name}#{revision}: {message}", instance.WorkableName, instance.WorkableRevision, message);

            instance.Log.Add($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} ERR: {message}");
            if(details != null)
                instance.Log.Add(details.ToString());
        }
    }
}