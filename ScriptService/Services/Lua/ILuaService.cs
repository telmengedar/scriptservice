using System.Collections.Generic;

namespace ScriptService.Services.Lua {
    
    /// <summary>
    /// service used to execute lua code
    /// </summary>
    public interface ILuaService {
        /// <summary>
        /// executes lua code
        /// </summary>
        /// <param name="code">code to execute</param>
        /// <param name="variables">variables for script execution</param>
        /// <returns>result of script</returns>
        object Execute(string code, IDictionary<string, object> variables);
    }
}