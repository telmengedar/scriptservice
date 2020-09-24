using System;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto.Scripts;
using ScriptService.Services.Scripts;

namespace ScriptService.Tests.Mocks {
    public class TestCompiler : IScriptCompiler {
        readonly IScriptParser parser = new ScriptParser();

        public Task<CompiledScript> CompileScript(long id, int? revision=null) {
            throw new NotImplementedException();
        }

        public Task<CompiledScript> CompileScript(string name, int? revision=null) {
            throw new NotImplementedException();
        }

        public IScript CompileCode(string code) {
            return parser.Parse(code);
        }

        public Task<IScript> CompileCodeAsync(string code) {
            return parser.ParseAsync(code);
        }
    }
}