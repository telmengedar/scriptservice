using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;

namespace ScriptService.Services {

    /// <summary>
    /// handles script instance data
    /// </summary>
    public interface ITaskService {

        /// <summary>
        /// creates new meta info for a script task
        /// </summary>
        /// <param name="workablename">name of workable</param>
        /// <param name="variables">variables used to start script</param>
        /// <param name="type">type of workable task is created for</param>
        /// <param name="workableid">id of workable</param>
        /// <param name="workablerevision">revision number of workable</param>
        /// <returns>task info</returns>
        WorkableTask CreateTask(WorkableType type, long workableid, int workablerevision, string workablename, IDictionary<string, object> variables);

        /// <summary>
        /// stores a task object
        /// </summary>
        /// <param name="task">task to store</param>
        /// <returns>task info</returns>
        Task StoreTask(WorkableTask task);
        
        /// <summary>
        /// get information about a script task
        /// </summary>
        /// <param name="id">id of script task</param>
        /// <returns>script task</returns>
        Task<WorkableTask> GetTask(Guid id);

        /// <summary>
        /// lists script tasks
        /// </summary>
        /// <param name="filter">filter for tasks to show</param>
        /// <returns>a page of script tasks which match the filter</returns>
        Task<Page<WorkableTask>> ListTasks(TaskFilter filter=null);

        /// <summary>
        /// finishes execution of a script task
        /// </summary>
        /// <param name="id">id of task which has finished</param>
        Task FinishTask(Guid id);

        /// <summary>
        /// cancels execution of a task
        /// </summary>
        /// <param name="id">id of task to cancel</param>
        Task<WorkableTask> CancelTask(Guid id);
    }
}