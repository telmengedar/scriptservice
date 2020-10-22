using System;
using ScriptService.Dto.Workflows.Nodes;

namespace ScriptService.Extensions {
    
    /// <summary>
    /// extension methods for <see cref="VariableOperation"/>
    /// </summary>
    public static class VariableOperationExtensions {
        static readonly string[] operators = {
            "=",
            "+", "-", "*", "/", "%",
            "&", "|", "^", "<<", ">>", "<<<", ">>>"
        };

        /// <summary>
        /// converts an operator to an operator string
        /// </summary>
        /// <param name="operation">operator to convert</param>
        /// <returns>string representation of operator</returns>
        public static string ToOperatorString(this VariableOperation operation) {
            if(operation<0||(int)operation>=operators.Length)
                throw new ArgumentException($"Operator '{operation}' not supported");

            return operators[(int) operation];
        }
    }
}