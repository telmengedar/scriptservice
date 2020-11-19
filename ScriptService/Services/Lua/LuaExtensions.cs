using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;

namespace ScriptService.Services.Lua {
    
    /// <summary>
    /// extensions for lua scripts
    /// </summary>
    public static class LuaExtensions {

        /// <summary>
        /// translates lua data types
        /// </summary>
        /// <param name="value">value to translate</param>
        public static object TranslateValue(object value) {
            if (value is IDictionary<string, object> dic) {
                dic.TranslateDictionary();
                return value;
            }

            if (value is Table table) {
                Dictionary<string, object> translation=new Dictionary<string, object>();
                foreach (DynValue key in table.Keys) {
                    if (key.Type != DataType.String)
                        continue;
                    translation[key.String] = TranslateValue(table[key]);
                }

                return translation;
            }

            if (value is DynValue dynvalue) {
                switch (dynvalue.Type) {
                case DataType.Boolean:
                    return dynvalue.Boolean;
                case DataType.Number:
                    return dynvalue.Number;
                case DataType.String:
                    return dynvalue.String;
                case DataType.UserData:
                    return dynvalue.UserData.Object;
                }
                return null;
            }
            
            return value;
        }

        /// <summary>
        /// translates lua data types in a dictionary
        /// </summary>
        /// <param name="variables">dictionary to translate</param>
        public static void TranslateDictionary(this IDictionary<string, object> variables) {
            foreach (string key in variables.Keys.ToArray())
                variables[key] = TranslateValue(variables[key]);
        }
    }
}