using System;
using Microsoft.Extensions.Logging;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;

namespace ScriptService.Services {

    /// <summary>
    /// routes logs to the service logger and to the script instance object
    /// </summary>
    public class WorkableLogger {
        readonly ILogger logger;
        readonly WorkableTask instance;

        /// <summary>
        /// creates a new <see cref="WorkableLogger"/>
        /// </summary>
        /// <param name="logger">logger to send logs to</param>
        /// <param name="instance">script instance to add logs to</param>
        public WorkableLogger(ILogger logger, WorkableTask instance) {
            this.logger = logger;
            this.instance = instance;
        }

        void Log(string severity, string message, string details = null) {
            switch (severity) {
            case "ERR":
                if (details != null)
                    logger.LogError($"{instance.WorkableName}#{instance.WorkableRevision}: {message}\n{details}");
                else
                    logger.LogError($"{instance.WorkableName}#{instance.WorkableRevision}: {message}");
                break;
            case "WRN":
                if(details != null)
                    logger.LogWarning($"{instance.WorkableName}#{instance.WorkableRevision}: {message}\n{details}");
                else
                    logger.LogWarning($"{instance.WorkableName}#{instance.WorkableRevision}: {message}");
                break;
            default:
                if(details != null)
                    logger.LogInformation($"{instance.WorkableName}#{instance.WorkableRevision}: {message}\n{details}");
                else
                    logger.LogInformation($"{instance.WorkableName}#{instance.WorkableRevision}: {message}");
                break;
            }

            
            instance.Log.Add($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} {severity}: {message}");
            if (details != null)
                instance.Log.Add(details);
        }

        /// <summary>
        /// logs an info
        /// </summary>
        /// <param name="message">info message</param>
        /// <param name="details">message details (optional)</param>
        public void Info(string message, string details = null) {
            Log("INF", message, details);
        }

        /// <summary>
        /// logs a warning
        /// </summary>
        /// <param name="message">warning message</param>
        /// <param name="details">message details (optional)</param>
        public void Warning(string message, string details = null) {
            Log("WRN", message, details);
        }

        /// <summary>
        /// logs an error
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="details">message details (optional)</param>
        public void Error(string message, string details = null) {
            Log("ERR", message, details);
        }

        /// <summary>
        /// logs an error
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="details">message details (optional)</param>
        public void Error(string message, Exception details = null) {
            Log("ERR", message, details?.ToString());
        }
    }
}