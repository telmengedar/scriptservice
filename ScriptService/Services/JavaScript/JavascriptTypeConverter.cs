﻿using System;
using System.Collections.Generic;
using Jint.Runtime.Interop;
using NightlyCode.AspNetCore.Services.Convert;
using ScriptService.Extensions;

namespace ScriptService.Services.JavaScript {

    /// <summary>
    /// converts values to .net objects
    /// </summary>
    public class JavascriptTypeConverter : ITypeConverter {

        public object Convert(object value, Type type, IFormatProvider formatProvider) {
            throw new NotImplementedException();
        }

        public bool TryConvert(object value, Type type, IFormatProvider formatProvider, out object converted) {
            if (value is IDictionary<string, object> dic) {
                converted = dic.Deserialize(type);
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