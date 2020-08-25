import { ImportDeclaration } from './importdeclaration';

/**
 * parameters for a start node
 */
export interface StartParameters {

    /**
     * list of imported host methods
     */
    imports: ImportDeclaration[]
}