import { IndexTransition } from './indexTransition';
import { NodeData } from './nodeData';
import { WorkflowData } from './workflowdata';

/**
 * complete workflow definition
 */
export interface WorkflowStructure extends WorkflowData {

    /**
     * nodes in workflow
     */
    nodes: NodeData[],

    /**
     * node transitions
     */
    transitions: IndexTransition[]
}