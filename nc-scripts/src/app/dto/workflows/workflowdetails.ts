import { Workflow } from './workflow';
import { WorkflowNode } from './workflownode';
import { Transition } from './transition';

/**
 * full workflow data with all nodes and transitions
 */
export interface WorkflowDetails extends Workflow {

    /**
     * nodes in workflow
     */
    nodes: WorkflowNode[],

    /**
     * transitions in workflow
     */
    transitions: Transition[]
}