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

    /**
     * path to property to patch
     */
    path: string,

    /**
     * value to use when patching property
     */
    value: any,

    /**
     * original path to property used for certain operations like 'move'
     */
    from?: string
}