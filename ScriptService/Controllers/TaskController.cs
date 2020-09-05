using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto.Tasks;
using ScriptService.Services;

namespace ScriptService.Controllers {

    /// <summary>
    /// provides endpoints for running tasks
    /// </summary>
    [Route("api/v1/tasks")]
    [ApiController]
    public class TaskController : ControllerBase {
        readonly ILogger<TaskController> logger;
        readonly ITaskService scripttaskservice;

        /// <summary>
        /// creates a new <see cref="TaskController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="scripttaskservice">access to script task service</param>
        public TaskController(ILogger<TaskController> logger, ITaskService scripttaskservice) {
            this.logger = logger;
            this.scripttaskservice = scripttaskservice;
        }

        /// <summary>
        /// get information about a script task
        /// </summary>
        /// <param name="id">id of script task</param>
        /// <returns>script task</returns>
        [HttpGet("{id}")]
        public Task<WorkableTask> GetTask(Guid id) {
            return scripttaskservice.GetTask(id);
        }

        /// <summary>
        /// lists script tasks
        /// </summary>
        /// <param name="filter">filter for tasks to show</param>
        /// <returns>a page of script tasks which match the filter</returns>
        [HttpGet]
        public Task<Page<WorkableTask>> ListTasks([FromQuery] TaskFilter filter) {
            return scripttaskservice.ListTasks(filter);
        }

        /// <summary>
        /// cancels execution of a task
        /// </summary>
        /// <param name="id">id of task to cancel</param>
        [HttpDelete("{id}")]
        public Task<WorkableTask> CancelTask(Guid id) {
            logger.LogInformation($"Cancelling task {id}");
            return scripttaskservice.CancelTask(id);
        }

    }
}
