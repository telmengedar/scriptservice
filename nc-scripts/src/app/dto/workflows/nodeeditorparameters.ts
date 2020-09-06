import { ImportDeclaration } from './importdeclaration';
import { NodeData } from './nodeData';

/**
 * parameters for node editor
 */
export interface NodeEditorParameters {

    /**
     * node to be edited
     */
    node: NodeData,

    /**
     * import declarations of current workflow
     */
    imports: ImportDeclaration[]
}