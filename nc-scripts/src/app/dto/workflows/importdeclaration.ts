/**
 * declaration of used host objects
 */
export interface ImportDeclaration {

    /**
     * variable to assign import to
     */
    variable: string,

    /**
     * type of import
     */
    type: string,

    /**
     * identifier to import
     */
    name: string
}