import { WorkflowStructure } from './workflowstructure';
import { TimeSpan } from '../timespan';

/**
 * parameters used to execute a workflow on server
 */
export interface ExecuteWorkflowParameters {
    id?: number,
    name?: string,
    workflow?: WorkflowStructure,
    parameters?: any,
    wait?: TimeSpan
}