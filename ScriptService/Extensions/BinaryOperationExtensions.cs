using System;
using System.Collections.Generic;
using ScriptService.Dto.Workflows.Nodes;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for <see cref="BinaryOperation"/>
    /// </summary>
    public static class BinaryOperationExtensions {
        static readonly string[] operators = {
            "+", "-", "*", "/", "%",
            "==", "!=", "<", "<=", ">", ">=", "~~", "!~",
            "&", "|", "^", "<<", ">>", "<<<", ">>>",
            "&&", "||", "^^"
        };

        /// <summary>
        /// converts an operator to an operator string
        /// </summary>
        /// <param name="operation">operator to convert</param>
        /// <returns>string representation of operator</returns>
        public static string ToOperatorString(this BinaryOperation operation) {
            if(operation<0||(int)operation>=operators.Length)
                throw new ArgumentException($"Operator '{operation}' not supported");

            return operators[(int) operation];
        }
    }
}