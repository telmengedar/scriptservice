import { WorkflowData } from './workflowdata';

/**
 * workflow stored in backend
 */
export interface Workflow extends WorkflowData {

    /**
     * workflow id
     */
    id: number,

    /**
     * workflow revision
     */
    revision: number
}