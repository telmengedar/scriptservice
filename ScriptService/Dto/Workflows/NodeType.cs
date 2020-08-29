namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// possible workflow node types
    /// </summary>
    public enum NodeType {

        /// <summary>
        /// start of workflow
        /// </summary>
        /// <remarks>
        /// only one start node can exist in a workflow
        /// </remarks>
        Start=0,

        /// <summary>
        /// simple node without any data
        /// used for simple branching without any actions
        /// </summary>
        Node=1,
        
        /// <summary>
        /// direct script code
        /// </summary>
        Expression=2,

        /// <summary>
        /// script call
        /// </summary>
        Script=3,

        /// <summary>
        /// binary math operation
        /// </summary>
        BinaryOperation,

        /// <summary>
        /// generates a constant or reads a value from state
        /// </summary>
        Value,

        /// <summary>
        /// calls a workflow
        /// </summary>
        Workflow,

        /// <summary>
        /// suspends execution of a workflow
        /// </summary>
        Suspend,

        /// <summary>
        /// calls a method on a host
        /// </summary>
        Call,

        /// <summary>
        /// iterates over every element of a collection
        /// </summary>
        Iterator,


    }
}