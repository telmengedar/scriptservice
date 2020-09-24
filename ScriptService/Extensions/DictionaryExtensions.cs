using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Scripting;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Errors;
using ScriptService.Services.Scripts;

namespace ScriptService.Extensions {

    /// <summary>
    /// extensions for dictionaries
    /// </summary>
    public static class DictionaryExtensions {

        /// <summary>
        /// deserializes a dictionary to a type
        /// </summary>
        /// <typeparam name="T">type to deserialize dictionary to</typeparam>
        /// <param name="dictionary">dictionary containing property values</param>
        /// <returns>deserialized object</returns>
        public static T Deserialize<T>(this IDictionary<string, object> dictionary) {
            return (T)Deserialize(dictionary, typeof(T));
        }

        /// <summary>
        /// deserializes a dictionary to a type
        /// </summary>
        /// <param name="dictionary">dictionary containing property values</param>
        /// <param name="targettype">type to deserialize dictionary to</param>
        /// <returns>deserialized object</returns>
        public static object Deserialize(this IDictionary<string, object> dictionary, Type targettype) {
            if (dictionary == null)
                return null;

            object data = Activator.CreateInstance(targettype);

            foreach(KeyValuePair<string, object> entries in dictionary) {
                if(entries.Value == null)
                    continue;

                PropertyInfo property = targettype.GetProperties().FirstOrDefault(p => string.Compare(p.Name, entries.Key, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0);
                if(property == null)
                    continue;

                if(property.PropertyType.IsArray) {
                    Array array;
                    Type elementtype = property.PropertyType.GetElementType();
                    if (elementtype == null)
                        // array without an element type should not happen ever
                        continue;

                    if(entries.Value is List<object> list) {
                        array = Array.CreateInstance(elementtype, list.Count);
                        for(int i = 0; i < list.Count; ++i) {
                            if (list[i] is IDictionary<string, object> dic)
                                array.SetValue(dic.Deserialize(elementtype), i);
                            else array.SetValue(Converter.Convert(list[i], elementtype), i);
                        }
                    }
                    else {
                        array = Array.CreateInstance(elementtype, 1);
                        if (entries.Value is IDictionary<string, object> dic)
                            array.SetValue(dic.Deserialize(elementtype), 0);
                        else array.SetValue(Converter.Convert(entries.Value, elementtype), 0);
                    }

                    property.SetValue(data, array);
                }
                else {
                    object value = Converter.Convert(entries.Value, property.PropertyType, true);
                    property.SetValue(data, value);
                }
            }

            return data;
        }

        /// <summary>
        /// builds node arguments from a dictionary
        /// </summary>
        /// <param name="data"></param>
        /// <returns>collection of prepared script arguments</returns>
        public static IEnumerable<ScriptArgument> BuildArguments(this IDictionary<string, object> data) {
            if(data == null)
                yield break;

            foreach(KeyValuePair<string, object> item in data) {
                if(item.Value is string stringitem) {
                    if(stringitem.StartsWith("$")) {
                        if(stringitem.Length == 1 || stringitem[1] == '$')
                            yield return new ScriptArgument(item.Key, ArgumentSourceType.Constant, stringitem.Substring(1));
                        else
                            yield return new ScriptArgument(item.Key, ArgumentSourceType.StateVariable, stringitem.Substring(1));

                    }
                    else
                        yield return new ScriptArgument(item.Key, ArgumentSourceType.Constant, stringitem);
                }
                else
                    yield return new ScriptArgument(item.Key, ArgumentSourceType.Constant, item.Value);
            }
        }

        /// <summary>
        /// builds arguments to be used by workables
        /// </summary>
        /// <param name="arguments">arguments specs from which to build workable arguments</param>
        /// <param name="state">state variables</param>
        /// <returns>workable variables</returns>
        public static Dictionary<string, object> BuildArguments(this ScriptArgument[] arguments, IDictionary<string, object> state) {
            Dictionary<string, object> scriptparameters = new Dictionary<string, object>();
            foreach(ScriptArgument parameter in arguments) {
                switch(parameter.Source) {
                case ArgumentSourceType.Constant:
                    scriptparameters[parameter.Name] = parameter.Value;
                    break;
                case ArgumentSourceType.StateVariable:
                    string variablename = parameter.Value?.ToString();
                    if(string.IsNullOrEmpty(variablename))
                        throw new WorkflowException("No variable name specified");
                    if(!state.ContainsKey(variablename))
                        throw new WorkflowException($"Variable '{variablename}' not found in workflow state");
                    scriptparameters[parameter.Name] = state[variablename];
                    break;
                default:
                    throw new WorkflowException($"Unknown argument source type '{parameter.Source}'");
                }
            }

            return scriptparameters;
        }

        /// <summary>
        /// builds argument scripts for workflow nodes
        /// </summary>
        /// <param name="arguments">arguments to use as code source</param>
        /// <param name="compiler">compiler used to compile scripts</param>
        /// <returns></returns>
        public static IDictionary<string, IScript> BuildArguments(this IDictionary<string, object> arguments, IScriptCompiler compiler) {
            if (arguments == null)
                return null;

            Dictionary<string, IScript> result=new Dictionary<string, IScript>();
            foreach ((string key, object value) in arguments) {
                if (value is string code)
                    result[key] = compiler.CompileCode(code) ?? new ConstantValueScript(null);
                else result[key] = new ConstantValueScript(value);
            }

            return result;
        }

        /// <summary>
        /// evaluates argument scripts and returns values to use as parameters
        /// </summary>
        /// <param name="arguments">arguments to evaluate</param>
        /// <param name="state">state to use as variable source</param>
        /// <param name="token">token used for cancellation</param>
        /// <returns>workable parameter values</returns>
        public static async Task<IDictionary<string, object>> EvaluateArguments(this IDictionary<string, IScript> arguments, IDictionary<string, object> state, CancellationToken token) {
            if (arguments == null)
                return new Dictionary<string, object>();

            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach((string key, IScript value) in arguments) {
                result[key] = await value.ExecuteAsync(state, token);
            }

            return result;
        }

    }
}