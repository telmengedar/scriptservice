using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Services.Scripts;

namespace ScriptService.Tests.Mocks {
    public class TestCompiler : IScriptCompiler {
        readonly IScriptParser parser = new ScriptParser();

        public Task<IScript> CompileCode(long id, int revision, string code) {
            return CompileCode(code);
        }

        public Task<IScript> CompileCode(string code) {
            return parser.ParseAsync(code);
        }
    }
}