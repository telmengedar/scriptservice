using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Extensions;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.Tasks {

    /// <summary>
    /// scans for tasks to run and executes them
    /// </summary>
    public class TaskScheduler : BackgroundService {
        readonly ILogger<TaskScheduler> logger;
        readonly IConfiguration configuration;
        readonly IScheduledTaskService scheduledtaskservice;
        readonly IWorkflowExecutionService workflowexecutor;
        readonly IScriptExecutionService scriptexecutor;
        readonly IWorkflowCompiler workflowcompiler;

        TimeSpan interval;

        /// <summary>
        /// creates a new <see cref="System.Threading.Tasks.TaskScheduler"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="configuration">access to configuration</param>
        /// <param name="scheduledtaskservice">access to scheduled task data</param>
        /// <param name="workflowexecutor">executes workflows</param>
        /// <param name="scriptexecutor">executes scripts</param>
        /// <param name="workflowcompiler">compiler for workflow data</param>
        public TaskScheduler(ILogger<TaskScheduler> logger, IConfiguration configuration, IScheduledTaskService scheduledtaskservice, IWorkflowExecutionService workflowexecutor, IScriptExecutionService scriptexecutor, IWorkflowCompiler workflowcompiler) {
            this.logger = logger;
            this.configuration = configuration;
            this.scheduledtaskservice = scheduledtaskservice;
            this.workflowexecutor = workflowexecutor;
            this.scriptexecutor = scriptexecutor;
            this.workflowcompiler = workflowcompiler;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            if (!TimeSpan.TryParse(configuration["TaskScheduler:Interval"], out interval))
                interval = TimeSpan.FromMinutes(5.0);

            while (!stoppingToken.IsCancellationRequested) {
                try {
                    await CheckTasks();
                }
                catch (Exception e) {
                    logger.LogError(e, "Error checking scheduled tasks");
                }

                await Task.Delay(interval, stoppingToken);
            }
        }

        async Task CheckTasks() {
            List<ScheduledTask> tasks=new List<ScheduledTask>();
            ScheduledTaskFilter filter = new ScheduledTaskFilter {
                DueTime = DateTime.Now
            };

            while(true) {
                Page<ScheduledTask> page = await scheduledtaskservice.List(filter);
                tasks.AddRange(page.Result);
                if (page.Result.Length == 0 || !page.Continue.HasValue)
                    break;
                filter.Continue = page.Continue;
            }

            foreach (ScheduledTask task in tasks) {
                switch (task.WorkableType) {
                case WorkableType.Workflow:
                    await workflowexecutor.Execute(await workflowcompiler.BuildWorkflow(task.WorkableName), new Dictionary<string, object>());
                    break;
                case WorkableType.Script:
                    await scriptexecutor.Execute(task.WorkableName, task.WorkableRevision);
                    break;
                }

                try {
                    await scheduledtaskservice.UpdateExecution(task.Id, task.NextExecutionTime());
                }
                catch (Exception e) {
                    logger.LogError(e, $"Unable to reschedule task '{task.Id}'");
                }
            }
        }
    }
}