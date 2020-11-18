using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;

namespace ScriptService.Services.Python {
    
    /// <summary>
    /// interprets and executes python code
    /// </summary>
    public interface IPythonService {
        
        /// <summary>
        /// parses code to an executable script object
        /// </summary>
        /// <param name="code">python code to parse</param>
        /// <returns>executable script object</returns>
        ScriptSource Parse(string code);

        /// <summary>
        /// executes a python script
        /// </summary>
        /// <param name="script">script to execute</param>
        /// <param name="variables">variables to provide to script</param>
        /// <returns>script result</returns>
        object Execute(ScriptSource script, IDictionary<string, object> variables);
    }
}