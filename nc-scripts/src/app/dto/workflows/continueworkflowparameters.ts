import { TimeSpan } from '../timespan';

/**
 * parameters to use when continuing suspended workflows
 */
export interface ContinueWorkflowParameters {

    /**
     * parameters for workflow continuation
     */
    parameters?: any,

    /**
     * time to wait for workflow to finish
     */
    wait?: TimeSpan
}