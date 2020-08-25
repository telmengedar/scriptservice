/**
 * code to execute directly on script server
 */
export interface ScopedCode {

    /**
     * scope under which to run code
     */
    scope: string,

    /**
     * code to execute
     */
    code: string,

    /**
     * name under which to run code
     */
    name: string
}