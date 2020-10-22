namespace ScriptService.Dto.Workflows.Nodes {
    
    /// <summary>
    /// operation to apply to the result variable of a workflow node
    /// </summary>
    public enum VariableOperation {
        
        /// <summary>
        /// assigns the value to the variable (default)
        /// </summary>
        Assign=0,
        
        /// <summary>
        /// add the value to the variable
        /// </summary>
        Add=1,

        /// <summary>
        /// subtract thea value from the existing
        /// </summary>
        Subtract = 2,

        /// <summary>
        /// multiply the value with the existing
        /// </summary>
        Multiply = 3,

        /// <summary>
        /// divide the existing value with the new
        /// </summary>
        Divide = 4,

        /// <summary>
        /// determines the remainder when dividing the existing value with the new value
        /// </summary>
        Modulo = 5,

        /// <summary>
        /// combines the two values bitwise using an AND operator
        /// </summary>
        BitAnd = 6,

        /// <summary>
        /// combines the two values bitwise using an OR operator
        /// </summary>
        BitOr = 7,

        /// <summary>
        /// combines the two values bitwise using an XOR operator
        /// </summary>
        BitXor = 8,

        /// <summary>
        /// bitwise shifts left the existing value by a specified number of steps
        /// </summary>
        ShiftLeft = 9,

        /// <summary>
        /// bitwise shifts right the existing value by a specified number of steps
        /// </summary>
        ShiftRight = 10,

        /// <summary>
        /// bitwise rolls left the existing value by a specified number of steps
        /// </summary>
        RollLeft = 11,

        /// <summary>
        /// bitwise rolls right the existing value by a specified number of steps
        /// </summary>
        RollRight = 12,
    }
}