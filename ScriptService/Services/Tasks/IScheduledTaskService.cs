using System;
using System.Threading.Tasks;
using ScriptService.Dto.Tasks;

namespace ScriptService.Services.Tasks {

    /// <summary>
    /// handles <see cref="ScheduledTask"/>
    /// </summary>
    public interface IScheduledTaskService : IRestDataService<ScheduledTask, ScheduledTaskData, ScheduledTaskFilter> {

        /// <summary>
        /// schedules task or resets schedule time
        /// </summary>
        /// <param name="id">id of task</param>
        /// <param name="time">time when task should run the next time, use null to disable task scheduling</param>
        Task Schedule(long id, DateTime? time);

        /// <summary>
        /// updates the last execution field with the current date
        /// </summary>
        /// <param name="id">id of task to update</param>
        /// <param name="nextexecution">time when task should run the next time, use null to disable task scheduling</param>
        Task UpdateExecution(long id, DateTime? nextexecution);
    }
}