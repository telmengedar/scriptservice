using System;
using System.Collections.Generic;
using System.Text;
using NightlyCode.Scripting.Parser;
using ScriptService.Errors;
using ScriptService.Services.Workflows;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for values
    /// </summary>
    public static class ValueExtensions {

        /// <summary>
        /// parses encoded string data
        /// </summary>
        /// <param name="data">data to parse</param>
        /// <returns>parsed string data</returns>
        public static string ParseString(ReadOnlySpan<char> data) {
            StringBuilder value = new StringBuilder();
            bool escaped = false;
            bool terminated = false;
            foreach (char character in data) {
                if (terminated)
                    throw new WorkflowException("Malformed string argument");

                if (escaped) {
                    switch (character) {
                    case 't':
                        value.Append('\t');
                        break;
                    case 'n':
                        value.Append('\n');
                        break;
                    case 'r':
                        value.Append('\r');
                        break;
                    case '0':
                        value.Append('\0');
                        break;
                    default:
                        value.Append(character);
                        break;
                    }
                    escaped = false;
                }
                else {
                    switch (character) {
                    case '\\':
                        escaped = true;
                        break;
                    case '"':
                        terminated = true;
                        break;
                    default:
                        value.Append(character);
                        break;
                    }
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// parses a value from a string
        /// </summary>
        /// <param name="data">data containing characters to parse</param>
        /// <param name="state">state variables</param>
        /// <returns>parsed value</returns>
        public static object ParseValue(ReadOnlySpan<char> data, StateVariableProvider state) {
            if (data.Length == 0)
                return null;

            int num = 0;
            int dec = 0;
            int misc = 0;

            foreach (char character in data) {
                if (char.IsDigit(character))
                    ++num;
                else if (character == '.')
                    ++dec;
                else ++misc;
            }

            if (misc > 0) {
                switch (data[0]) {
                case '"':
                    return ParseString(data.Slice(1));
                case '$':
                    if (data.Length > 1 && data[1] == '$')
                        return new string(data.Slice(1));
                    else {
                        string name = new string(data.Slice(1));
                        if (state.TryGetValue(name, out object value))
                            return value;
                        throw new WorkflowException($"Variable '{name}' not found in state");
                    }
                default:
                    return new string(data);
                }
            }

            switch (dec) {
            case 0:
                return long.Parse(data);
            case 1:
                return decimal.Parse(data);
            default:
                return new string(data);
            }
        }

        /// <summary>
        /// determines argument value to use
        /// </summary>
        /// <param name="value">value to analyse</param>
        /// <param name="state">state variables</param>
        /// <returns>value to use</returns>
        public static object DetermineValue(this object value, StateVariableProvider state) {
            if (value is string stringvalue)
                return ParseValue(stringvalue, state);

            return value;
        }

        /// <summary>
        /// tries to convert a value to boolean
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns>boolean representation</returns>
        public static bool ToBoolean(this object value) {
            if(value == null)
                return false;

            if(value is bool b)
                return b;

            if(value is string s)
                return s != "";

            if(value is IComparable comparable)
                return comparable.CompareTo(Activator.CreateInstance(value.GetType())) != 0;
            return false;
        }

        /// <summary>
        /// get bit mask for specified type
        /// </summary>
        /// <param name="masktype">type for which to generate bitmask</param>
        /// <param name="numberofbits">number of bits to set</param>
        /// <returns>bitmask</returns>
        public static object GetMask(Type masktype, int numberofbits) {
            object mask = Activator.CreateInstance(masktype);
            for(int i = 0; i < numberofbits; ++i) {
                mask = (dynamic)mask << 1;
                mask = (dynamic)mask | 1;
            }

            return mask;
        }

        /// <summary>
        /// get number of set bits of a value
        /// </summary>
        /// <param name="value">value to analyse</param>
        /// <returns>number of set bits in the value</returns>
        /// <exception cref="WorkflowException">in case value is not a supported value type</exception>
        public static int GetNumberOfBits(this object value) {
            switch (value) {
            case int _:
            case uint _:
                return 32;
            case long _:
            case ulong _:
                return 64;
            case short _:
            case ushort _:
                return 16;
            case byte _:
            case sbyte _:
                return 8;
            default:
                throw new WorkflowException($"'{value.GetType()}' not supported for bit operation");
            }
        }

    }
}