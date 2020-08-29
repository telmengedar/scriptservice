namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// binary operation to execute
    /// </summary>
    public enum BinaryOperation {

        /// <summary>
        /// add two values
        /// </summary>
        Add=0,

        /// <summary>
        /// subtract a value from another
        /// </summary>
        Subtract = 1,

        /// <summary>
        /// multiply two values
        /// </summary>
        Multiply = 2,

        /// <summary>
        /// divide a value by another
        /// </summary>
        Divide = 3,

        /// <summary>
        /// remainder of division
        /// </summary>
        Modulo = 4,

        /// <summary>
        /// compare for equality
        /// </summary>
        Equality = 5,

        /// <summary>
        /// compare for inequality
        /// </summary>
        Inequality = 6,

        /// <summary>
        /// determines whether a value is less than another value
        /// </summary>
        Less = 7,

        /// <summary>
        /// determines whether a value is less or equal to another value
        /// </summary>
        LessOrEqual = 8,

        /// <summary>
        /// determines whether a value is greater than another value
        /// </summary>
        Greater = 9,

        /// <summary>
        /// determines whether a value is greater or equal to another value
        /// </summary>
        GreaterOrEqual = 10,

        /// <summary>
        /// determines whether a value matches another value
        /// </summary>
        Matches = 11,

        /// <summary>
        /// determines whether a value does not match another value
        /// </summary>
        MatchesNot = 12,

        /// <summary>
        /// combines two values bitwise using an AND operator
        /// </summary>
        BitAnd = 13,

        /// <summary>
        /// combines two values bitwise using an OR operator
        /// </summary>
        BitOr = 14,

        /// <summary>
        /// combines two values bitwise using an XOR operator
        /// </summary>
        BitXor = 15,

        /// <summary>
        /// bitwise shifts left a value by a specified number of steps
        /// </summary>
        ShiftLeft = 16,

        /// <summary>
        /// bitwise shifts right a value by a specified number of steps
        /// </summary>
        ShiftRight = 17,

        /// <summary>
        /// bitwise rolls left a value by a specified number of steps
        /// </summary>
        RollLeft = 18,

        /// <summary>
        /// bitwise rolls right a value by a specified number of steps
        /// </summary>
        RollRight = 19,

        /// <summary>
        /// logically compares two values using an AND operator
        /// </summary>
        LogicalAnd = 20,

        /// <summary>
        /// logically compares two values using an OR operator
        /// </summary>
        LogicalOr = 21,

        /// <summary>
        /// logically compares two values using a XOR operator
        /// </summary>
        LogicalXor = 22
    }
}