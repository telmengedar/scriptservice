/**
 * transition from one {@link WorkflowNode} to another
 */
export interface Transition {

    /**
     * id of {@link WorkflowNode} this transition originates from
     */
    originId: string,

    /**
     * id of {@link WorkflowNode} this transition targets
     */
    targetId: string,

    /**
     * condition to match for this {@link Transition} to be executed
     */
    condition?: string,

    /**
     * determines whether transition is used for error handling
     */
    error: boolean
}