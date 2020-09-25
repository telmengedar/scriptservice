using System.Threading.Tasks;
using Esprima.Ast;

namespace ScriptService.Services.JavaScript {

    /// <summary>
    /// parser for javascript code
    /// </summary>
    public interface IJavascriptParser {

        /// <summary>
        /// parses javascript code to an executable script
        /// </summary>
        /// <param name="code">code to parse</param>
        /// <returns>parsed code</returns>
        Script Parse(string code);

        /// <summary>
        /// parses javascript code to an executable script
        /// </summary>
        /// <param name="code">code to parse</param>
        /// <returns>parsed code</returns>
        Task<Script> ParseAsync(string code);
    }
}