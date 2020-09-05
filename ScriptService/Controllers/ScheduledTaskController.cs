using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto.Patches;
using ScriptService.Dto.Tasks;
using ScriptService.Services.Tasks;

namespace ScriptService.Controllers {

    /// <summary>
    /// provides endpoints for task scheduling
    /// </summary>
    [Route("api/v1/scheduler")]
    [ApiController]
    public class ScheduledTaskController : Controller {
        readonly ILogger<ScheduledTaskController> logger;
        readonly IScheduledTaskService scheduledtaskservice;

        /// <summary>
        /// creates a new <see cref="ScheduledTaskController"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="scheduledtaskservice">access to scheduled task data</param>
        public ScheduledTaskController(ILogger<ScheduledTaskController> logger, IScheduledTaskService scheduledtaskservice) {
            this.logger = logger;
            this.scheduledtaskservice = scheduledtaskservice;
        }

        /// <summary>
        /// creates a new task
        /// </summary>
        /// <param name="task">data for task to create</param>
        /// <returns>id of created task</returns>
        [HttpPost]
        public async Task<ScheduledTask> CreateScript(ScheduledTaskData task) {
            logger.LogInformation($"Creating scheduled task {task.Name}");
            return await scheduledtaskservice.GetById(await scheduledtaskservice.Create(task));
        }

        /// <summary>
        /// get a task by id
        /// </summary>
        /// <param name="taskid">id of task to get</param>
        /// <returns>task with the specified id</returns>
        [HttpGet("{taskid}")]
        public Task<ScheduledTask> GetScript(long taskid) {
            return scheduledtaskservice.GetById(taskid);
        }

        /// <summary>
        /// lists scripts matching a criteria
        /// </summary>
        /// <param name="filter">filter for scripts to match</param>
        /// <returns>a page of scripts which match the filter</returns>
        [HttpGet]
        public Task<Page<ScheduledTask>> ListScripts([FromQuery] ScheduledTaskFilter filter) {
            return scheduledtaskservice.List(filter);
        }

        /// <summary>
        /// patches properties of a task
        /// </summary>
        /// <param name="taskid">id of task to patch</param>
        /// <param name="patches">patches to apply</param>
        [HttpPatch("{taskid}")]
        public async Task<ScheduledTask> PatchScript(long taskid, [FromBody] PatchOperation[] patches) {
            logger.LogInformation($"Patching scheduled task {taskid}");
            await scheduledtaskservice.Update(taskid, patches);
            return await scheduledtaskservice.GetById(taskid);
        }

        /// <summary>
        /// deletes an existing task
        /// </summary>
        /// <param name="taskid">id of task to delete</param>
        [HttpDelete("{taskid}")]
        public Task DeleteScript(long taskid) {
            logger.LogInformation($"Deleting scheduled task {taskid}");
            return scheduledtaskservice.Delete(taskid);
        }

    }
}