using System;
using ScriptService.Dto.Tasks;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for <see cref="ScheduledTask"/>
    /// </summary>
    public static class ScheduledTaskExtensions {

        /// <summary>
        /// get next execution time
        /// </summary>
        /// <param name="task">task for which to compute next execution time</param>
        /// <param name="referencetime">time to use as reference base (optional)</param>
        /// <returns>next execution time or null if no next execution is scheduled</returns>
        public static DateTime? NextExecutionTime(this ScheduledTaskData task, DateTime? referencetime=null) {
            if (!task.Interval.HasValue)
                return null;

            referencetime??=DateTime.Now;
            return ExecutionTime(task, referencetime.Value+task.Interval.Value);
        }

        /// <summary>
        /// get next execution time
        /// </summary>
        /// <param name="task">task for which to compute next execution time</param>
        /// <returns>next execution time or null if no next execution is scheduled</returns>
        public static DateTime? FirstExecutionTime(this ScheduledTaskData task) {
            return ExecutionTime(task, DateTime.Now);
        }

        static DateTime? ExecutionTime(this ScheduledTaskData task, DateTime time) {
            ScheduledDays dayfilter = task.Days;
            if(dayfilter == ScheduledDays.None)
                dayfilter = ScheduledDays.All;

            while((time.DayOfWeek.ToScheduledDays() & dayfilter) == ScheduledDays.None)
                time += TimeSpan.FromDays(1.0);

            return time;
        }
    }
}