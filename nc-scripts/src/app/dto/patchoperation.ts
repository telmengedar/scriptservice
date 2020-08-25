/**
 * patch which can get applied to an entity
 */
export interface PatchOperation {
    /**
     * operation type
     *  replace:    set a value
     *  add:        add an item to an array
     *  remove:     remove an item from an array
     */
    op: string,
    path: string,
    value: any,
    from?: string
}