namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// binary operation to execute
    /// </summary>
    public enum BinaryOperation {

        /// <summary>
        /// add two values
        /// </summary>
        Add,

        /// <summary>
        /// subtract a value from another
        /// </summary>
        Subtract,

        /// <summary>
        /// multiply two values
        /// </summary>
        Multiply,

        /// <summary>
        /// divide a value by another
        /// </summary>
        Divide,

        /// <summary>
        /// remainder of division
        /// </summary>
        Modulo,

        /// <summary>
        /// compare for equality
        /// </summary>
        Equality,

        /// <summary>
        /// compare for inequality
        /// </summary>
        Inequality,

        /// <summary>
        /// determines whether a value is less than another value
        /// </summary>
        Less,

        /// <summary>
        /// determines whether a value is less or equal to another value
        /// </summary>
        LessOrEqual,

        /// <summary>
        /// determines whether a value is greater than another value
        /// </summary>
        Greater,

        /// <summary>
        /// determines whether a value is greater or equal to another value
        /// </summary>
        GreaterOrEqual,

        /// <summary>
        /// determines whether a value matches another value
        /// </summary>
        Matches,

        /// <summary>
        /// determines whether a value does not match another value
        /// </summary>
        MatchesNot,

        /// <summary>
        /// combines two values bitwise using an AND operator
        /// </summary>
        BitAnd,

        /// <summary>
        /// combines two values bitwise using an OR operator
        /// </summary>
        BitOr,

        /// <summary>
        /// combines two values bitwise using an XOR operator
        /// </summary>
        BitXor,

        /// <summary>
        /// bitwise shifts left a value by a specified number of steps
        /// </summary>
        ShiftLeft,

        /// <summary>
        /// bitwise shifts right a value by a specified number of steps
        /// </summary>
        ShiftRight,

        /// <summary>
        /// bitwise rolls left a value by a specified number of steps
        /// </summary>
        RollLeft,

        /// <summary>
        /// bitwise rolls right a value by a specified number of steps
        /// </summary>
        RollRight,

        /// <summary>
        /// logically compares two values using an OR operator
        /// </summary>
        LogicalOr,

        /// <summary>
        /// logically compares two values using an AND operator
        /// </summary>
        LogicalAnd,

        /// <summary>
        /// logically compares two values using a XOR operator
        /// </summary>
        LogicalXor
    }
}