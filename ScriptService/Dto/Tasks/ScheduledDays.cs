using System;

namespace ScriptService.Dto.Tasks {

    /// <summary>
    /// days when tasks are scheduled
    /// </summary>
    [Flags]
    public enum ScheduledDays {

        /// <summary>
        /// no days selected. Usually leads to all days scheduled except when used as filter
        /// </summary>
        None=0,

        /// <summary>
        /// monday
        /// </summary>
        Monday=1,

        /// <summary>
        /// tuesday
        /// </summary>
        Tuesday=2,

        /// <summary>
        /// wednesday
        /// </summary>
        Wednesday=4,

        /// <summary>
        /// thursday
        /// </summary>
        Thursday=8,

        /// <summary>
        /// friday
        /// </summary>
        Friday=16,

        /// <summary>
        /// saturday
        /// </summary>
        Saturday=32,

        /// <summary>
        /// sunday
        /// </summary>
        Sunday=64,

        /// <summary>
        /// all days selected
        /// </summary>
        All=127
    }
}