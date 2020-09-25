using System;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Scripts;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Scripts;

namespace ScriptService.Tests.Mocks {
    public class TestCompiler : IScriptCompiler {
        readonly IScriptParser parser = new ScriptParser();
        readonly IJavascriptParser jsparser = new JavascriptParser();

        public Task<CompiledScript> CompileScriptAsync(long id, int? revision=null) {
            throw new NotImplementedException();
        }

        public Task<CompiledScript> CompileScriptAsync(string name, int? revision=null) {
            throw new NotImplementedException();
        }

        public IScript CompileCode(string code, ScriptLanguage language) {
            if(string.IsNullOrEmpty(code))
                return null;

            switch(language) {
            case ScriptLanguage.NCScript:
                return parser.Parse(code);
            case ScriptLanguage.JavaScript:
                return new JavaScript(jsparser.Parse(code), null);
            default:
                throw new ArgumentException($"Unsupported script language '{language}'");
            }
        }

        public async Task<IScript> CompileCodeAsync(string code, ScriptLanguage language) {
            if(string.IsNullOrEmpty(code))
                return null;

            switch(language) {
            case ScriptLanguage.NCScript:
                return await parser.ParseAsync(code);
            case ScriptLanguage.JavaScript:
                return new JavaScript(await jsparser.ParseAsync(code), null);
            default:
                throw new ArgumentException($"Unsupported script language '{language}'");
            }
        }
    }
}