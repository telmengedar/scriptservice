using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;
using ScriptService.Services.Workflows;

namespace ScriptService.Services {

    /// <summary>
    /// service used to execute workflows
    /// </summary>
    public interface IWorkflowExecutionService {

        /// <summary>
        /// continues execution of a suspended workflow
        /// </summary>
        /// <param name="taskid">id of task which was suspended</param>
        /// <param name="variables">variables to push to state</param>
        /// <param name="wait">time to wait for workflow to finish</param>
        /// <returns>task information</returns>
        Task<WorkableTask> Continue(Guid taskid, IDictionary<string, object> variables, TimeSpan? wait = null);

        /// <summary>
        /// executes a workflow instance
        /// </summary>
        /// <param name="workflow">workflow to execute</param>
        /// <param name="wait">time to wait for result</param>
        /// <returns>workflow result</returns>
        Task<WorkableTask> Execute(WorkflowInstance workflow, TimeSpan? wait = null);

        /// <summary>
        /// executes a workflow instance
        /// </summary>
        /// <param name="workflow">workflow to execute</param>
        /// <param name="tasklogger">logger to use</param>
        /// <param name="token">token to use for cancellation</param>
        /// <returns>workflow result</returns>
        Task<object> Execute(WorkflowInstance workflow, WorkableLogger tasklogger, CancellationToken token);
    }
}