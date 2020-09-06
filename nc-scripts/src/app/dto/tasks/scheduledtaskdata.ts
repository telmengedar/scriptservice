import { WorkableType } from './workabletype';
import { ScheduledDays } from './scheduleddays';
import { TimeSpan } from '../timespan';

/**
 * data of scheduled task
 */
export interface ScheduledTaskData {

    /**
     * name of scheduled task
     */
    name: string,

    /**
     * type of workable to execute
     */
    workableType: WorkableType,

    /**
     * name of workable to execute
     */
    workableName: string

    /**
     * revision of workable to execute (optional)
     */
    workableRevision?: number,

    /**
     * days on which to execute task
     */
    days: ScheduledDays,

    /**
     * interval used to repeat execution
     */
    interval?: TimeSpan
}