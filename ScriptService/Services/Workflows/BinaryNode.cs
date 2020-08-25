using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Errors;
using ScriptService.Extensions;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// node executing a binary operation
    /// </summary>
    public class BinaryNode : InstanceNode {

        /// <summary>
        /// creates a new <see cref="BinaryNode"/>
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="parameters">parameters for operation</param>
        public BinaryNode(string name, BinaryOpParameters parameters) 
        : base(name)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for operation
        /// </summary>
        public BinaryOpParameters Parameters { get; }

        /// <inheritdoc />
        public override Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            object lhs = Parameters.Lhs.DetermineValue(state);
            object rhs = Parameters.Rhs.DetermineValue(state);

            switch (Parameters.Operation) {
            case BinaryOperation.Add:
                return Task.FromResult<object>((dynamic) lhs + (dynamic) rhs);
            case BinaryOperation.Subtract:
                return Task.FromResult<object>((dynamic)lhs - (dynamic)rhs);
            case BinaryOperation.Multiply:
                return Task.FromResult<object>((dynamic)lhs * (dynamic)rhs);
            case BinaryOperation.Divide:
                return Task.FromResult<object>((dynamic)lhs / (dynamic)rhs);
            case BinaryOperation.Modulo:
                return Task.FromResult<object>((dynamic)lhs % (dynamic)rhs);
            case BinaryOperation.Equality:
                return Task.FromResult<object>((dynamic)lhs == (dynamic)rhs);
            case BinaryOperation.Inequality:
                return Task.FromResult<object>((dynamic)lhs != (dynamic)rhs);
            case BinaryOperation.Less:
                return Task.FromResult<object>((dynamic)lhs < (dynamic)rhs);
            case BinaryOperation.LessOrEqual:
                return Task.FromResult<object>((dynamic)lhs <= (dynamic)rhs);
            case BinaryOperation.Greater:
                return Task.FromResult<object>((dynamic)lhs > (dynamic)rhs);
            case BinaryOperation.GreaterOrEqual:
                return Task.FromResult<object>((dynamic)lhs >= (dynamic)rhs);
            case BinaryOperation.Matches:
                return Task.FromResult<object>(Matches(lhs, rhs));
            case BinaryOperation.MatchesNot:
                return Task.FromResult<object>(!Matches(lhs, rhs));
            case BinaryOperation.BitAnd:
                return Task.FromResult<object>((dynamic)lhs & (dynamic)rhs);
            case BinaryOperation.BitOr:
                return Task.FromResult<object>((dynamic)lhs | (dynamic)rhs);
            case BinaryOperation.BitXor:
                return Task.FromResult<object>((dynamic)lhs ^ (dynamic)rhs);
            case BinaryOperation.ShiftLeft:
                return Task.FromResult<object>((dynamic)lhs << (dynamic)rhs);
            case BinaryOperation.ShiftRight:
                return Task.FromResult<object>((dynamic)lhs >> (dynamic)rhs);
            case BinaryOperation.RollLeft:
                return Task.FromResult(RollLeft(lhs, rhs));
            case BinaryOperation.RollRight:
                return Task.FromResult(RollRight(lhs, rhs));
            case BinaryOperation.LogicalOr:
                return Task.FromResult<object>(lhs.ToBoolean() || rhs.ToBoolean());
            case BinaryOperation.LogicalAnd:
                return Task.FromResult<object>(lhs.ToBoolean() && rhs.ToBoolean());
            case BinaryOperation.LogicalXor:
                return Task.FromResult<object>(lhs.ToBoolean() != rhs.ToBoolean());
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        bool Matches(object lhs, object rhs) {
            if(!(rhs is string pattern))
                throw new WorkflowException("Matching pattern must be a regex string");

            string value = lhs?.ToString();
            if(value == null)
                return false;

            return Regex.IsMatch(value, pattern);
        }

        object RollLeft(object lhs, object rhs) {
            int steps = Converter.Convert<int>(rhs);

            int numberofbits = lhs.GetNumberOfBits();
            steps = steps % numberofbits;
            if(steps == 0)
                return lhs;

            object mask = ValueExtensions.GetMask(lhs.GetType(), steps);
            return ((dynamic)lhs << steps) | (((dynamic)lhs >> (numberofbits - steps)) & (dynamic)mask);
        }

        object RollRight(object lhs, object rhs) {
            object value = lhs;
            int steps = Converter.Convert<int>(rhs);

            int numberofbits = value.GetNumberOfBits();
            steps = steps % numberofbits;
            if(steps == 0)
                return value;

            object mask = ValueExtensions.GetMask(value.GetType(), numberofbits - steps);
            return (((dynamic)value >> steps) & (dynamic)mask) | ((dynamic)value << (numberofbits - steps));
        }
    }
}