import { NodeType } from './nodetype';

/**
 * data of a workflow node
 */
export interface NodeData {

    /**
     * name of node
     */
    name: string,

    /**
     * name of group node is part of
     */
    group?: string,

    /**
     * type of node
     */
    type: NodeType,

    /**
     * node parameters
     */
    parameters?: any,

    /**
     * variable to assign with return value of node
     */
    variable?: string
}