using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Providers;
using ScriptService.Services.Cache;

namespace ScriptService.Services.Scripts {

    /// <inheritdoc />
    public class ScriptCompiler : IScriptCompiler {
        readonly ILogger<ScriptCompiler> logger;
        readonly IScriptParser parser;
        readonly ICacheService cache;

        /// <summary>
        /// creates a new <see cref="ScriptCompiler"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="parser">parser used to parse scripts</param>
        /// <param name="cache">access to object cache</param>
        /// <param name="methodprovider">provides managed method hosts to scripts</param>
        public ScriptCompiler(ILogger<ScriptCompiler> logger, IScriptParser parser, ICacheService cache, IImportProvider methodprovider) {
            this.parser = parser;
            this.cache = cache;
            this.logger = logger;
            parser.ImportProvider = methodprovider;
        }

        /// <inheritdoc />
        public async Task<IScript> CompileCode(long id, int revision, string code) {
            IScript script = cache.GetObject<IScript, long>(id, revision);
            if (script != null)
                return script;

            logger.LogInformation($"Parsing script '{id}.{revision}'");
            script = await CompileCode(code);
            cache.StoreObject(id, revision, script);
            return script;
        }

        /// <inheritdoc />
        public Task<IScript> CompileCode(string code) {
            if (string.IsNullOrEmpty(code))
                return Task.FromResult<IScript>(null);
            return parser.ParseAsync(code);
        }
    }
}