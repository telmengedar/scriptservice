/**
 * transition using node indices as arguments
 */
export interface IndexTransition {

    /**
     * index of node where transition originates from
     */
    originIndex: number,

    /**
     * index of node which is targeted by transition
     */
    targetIndex: number,

    /**
     * condition for transition to execute
     */
    condition?: string,

    /**
     * determines whether transition is used for error handling
     */
    error: boolean
}