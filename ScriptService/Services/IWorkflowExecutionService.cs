using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptService.Dto.Tasks;
using ScriptService.Dto.Workflows;

namespace ScriptService.Services {

    /// <summary>
    /// service used to execute workflows
    /// </summary>
    public interface IWorkflowExecutionService {

        /// <summary>
        /// executes a workflow by specifying its id
        /// </summary>
        /// <param name="id">id of workflow to execute</param>
        /// <param name="variables">variables to pass to workflow</param>
        /// <param name="wait">time to wait for workflow to finish</param>
        /// <returns>task information</returns>
        Task<WorkableTask> Execute(long id, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// executes a workflow by specifying its name
        /// </summary>
        /// <param name="name">name of workflow to execute</param>
        /// <param name="variables">variables to pass to workflow</param>
        /// <param name="wait">time to wait for workflow to finish</param>
        /// <returns>task information</returns>
        Task<WorkableTask> Execute(string name, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// executes a workflow by specifying its name
        /// </summary>
        /// <param name="workflow">workflow to execute</param>
        /// <param name="variables">variables to pass to workflow</param>
        /// <param name="wait">time to wait for workflow to finish</param>
        /// <returns>task information</returns>
        Task<WorkableTask> Execute(WorkflowStructure workflow, IDictionary<string, object> variables = null, TimeSpan? wait = null);

        /// <summary>
        /// continues execution of a suspended workflow
        /// </summary>
        /// <param name="taskid">id of task which was suspended</param>
        /// <param name="variables">variables to push to state</param>
        /// <param name="wait">time to wait for workflow to finish</param>
        /// <returns>task information</returns>
        Task<WorkableTask> Continue(Guid taskid, IDictionary<string, object> variables, TimeSpan? wait = null);
    }
}