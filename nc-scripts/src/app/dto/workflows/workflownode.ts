import { NodeData } from './nodeData';

/**
 * node in a workflow
 */
export interface WorkflowNode extends NodeData {

    /**
     * id of node
     */
    id: string
}