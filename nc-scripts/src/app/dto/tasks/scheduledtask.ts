import { ScheduledTaskData } from './scheduledtaskdata';

/**
 * serialized scheduled task
 */
export interface ScheduledTask extends ScheduledTaskData {

    /**
     * id of planned task
     */
    id?: number,

    /**
     * date when task is targeted to get executed next
     */
    target?: Date
}