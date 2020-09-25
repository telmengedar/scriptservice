using System.Threading.Tasks;
using Esprima;
using Esprima.Ast;

namespace ScriptService.Services.JavaScript {

    /// <inheritdoc />
    public class JavascriptParser : IJavascriptParser {

        /// <inheritdoc />
        public Task<Script> ParseAsync(string code) {
            return Task.Run(() => Parse(code));
        }

        /// <inheritdoc />
        public Script Parse(string code) {
            return new JavaScriptParser(code).ParseScript();
        }

    }
}