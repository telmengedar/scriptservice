/**
 * descriptor for a column
 */
export interface Column {

    /**
     * name of column
     */
    name: string,

    /**
     * text to display
     */
    display: string,

    /**
     * format to use
     */
    format?: string
}