using System;
using ScriptService.Dto.Tasks;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for <see cref="DayOfWeek"/>
    /// </summary>
    public static class DayOfWeekExtensions {

        /// <summary>
        /// converts a weekday to a scheduled days filter
        /// </summary>
        /// <param name="weekday">day of week to convert</param>
        /// <returns>scheduled days formatted data</returns>
        public static ScheduledDays ToScheduledDays(this DayOfWeek weekday) {
            switch (weekday) {
            case DayOfWeek.Monday:
                return ScheduledDays.Monday;
            case DayOfWeek.Tuesday:
                return ScheduledDays.Tuesday;
            case DayOfWeek.Wednesday:
                return ScheduledDays.Wednesday;
            case DayOfWeek.Thursday:
                return ScheduledDays.Thursday;
            case DayOfWeek.Friday:
                return ScheduledDays.Friday;
            case DayOfWeek.Saturday:
                return ScheduledDays.Saturday;
            case DayOfWeek.Sunday:
                return ScheduledDays.Sunday;
            default:
                return ScheduledDays.None;
            }
        }
    }
}