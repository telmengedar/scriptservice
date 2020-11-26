using System;
using System.Collections.Generic;
using Jint.Runtime.Interop;
using NightlyCode.AspNetCore.Services.Convert;
using ScriptService.Extensions;

namespace ScriptService.Services.JavaScript {

    /// <summary>
    /// converts values to .net objects
    /// </summary>
    public class JavascriptTypeConverter : ITypeConverter {

        /// <inheritdoc />
        public object Convert(object value, Type type, IFormatProvider formatProvider) {
            if (type.IsInstanceOfType(value))
                return value;
            throw new ArgumentException($"Cant convert '{value}' to '{type.Name}'");
        }

        /// <inheritdoc />
        public bool TryConvert(object value, Type type, IFormatProvider formatProvider, out object converted) {
            if (value is IDictionary<string, object> dic) {
                converted = type == typeof(IDictionary<string, object>) ? dic : dic.Deserialize(type);
                return true;
            }

            try {
                converted = Converter.Convert(value, type);
                return true;
            }
            catch {
                
            }
            
            converted = null;
            return false;
        }
    }
}