/**
 * supported node types in workflows
 */
export enum NodeType {

    /**
     * start of workflow
     */
    Start=0,

    /**
     * simple node without any data
     * used for simple branching without any actions
     */
    Node=1,

    /**
     * embedded script code
     */
    Expression=2,

    /**
     * script call
     */
    Script=3,

    /**
     * binary math or logical operation
     */
    BinaryOperation=4,

    /**
     * generates a constant or reads a value from state
     */
    Value=5,

    /**
     * calls a workflow
     */
    Workflow=6,

    /**
     * suspends execution of a workflow
     */
    Suspend=7,

    /**
     * calls a method on a type or host
     */
    Call=8,

    /**
     * iterates over every element of a collection using loop transitions
     */
    Iterator=9,

    /**
     * node used to log messages
     */
    Log=10
}

export namespace NodeType {

    export function getName(type: any): string {
        if(typeof type === "number")
            return NodeType[type];

        if(typeof type === "string")
        {
            let num=parseInt(type)
            if(num>=0)
                return NodeType[num];
        }

        return type.toString();
    }

    export function getValue(type: any): NodeType {
        if(type as string) {
            let num=parseInt(type);
            if(num>=0)
                return num;
            return NodeType[type as string];
        }
        return type;
    }
}