using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Scripts;
using ScriptService.Services.Cache;
using ScriptService.Services.Scripts.Extensions;

namespace ScriptService.Services.Scripts {

    /// <inheritdoc />
    public class ScriptCompiler : IScriptCompiler {
        readonly ILogger<ScriptCompiler> logger;
        readonly IScriptService scriptservice;
        readonly IScriptParser parser;
        readonly ICacheService cache;
        readonly IArchiveService archive;

        /// <summary>
        /// creates a new <see cref="ScriptCompiler"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="parser">parser used to parse scripts</param>
        /// <param name="cache">access to object cache</param>
        /// <param name="methodprovider">provides managed method hosts to scripts</param>
        /// <param name="scriptservice">used to load scripts if not found in cache</param>
        /// <param name="archive">archive used to load revisions</param>
        public ScriptCompiler(ILogger<ScriptCompiler> logger, IScriptParser parser, ICacheService cache, IMethodProviderService methodprovider, IScriptService scriptservice, IArchiveService archive) {
            this.parser = parser;
            this.cache = cache;
            this.scriptservice = scriptservice;
            this.archive = archive;
            this.logger = logger;
            parser.Extensions.AddExtensions(typeof(Math));
            parser.Extensions.AddExtensions<ScriptEnumerations>();
            parser.ImportProvider = methodprovider;
        }

        async Task<CompiledScript> Parse(Script scriptdata) {
            logger.LogInformation($"Parsing script '{scriptdata.Id}.{scriptdata.Revision}'");
            IScript script = await CompileCode(scriptdata.Code);

            return new CompiledScript {
                Id = scriptdata.Id,
                Revision = scriptdata.Revision,
                Name = scriptdata.Name,
                Instance = script
            };
        }

        /// <inheritdoc />
        public async Task<CompiledScript> CompileScript(long id, int? revision=null) {
            CompiledScript script = cache.GetObject<CompiledScript, long>(id, revision??0);
            if (script != null)
                return script;

            Script scriptdata = await scriptservice.GetScript(id);
            if (revision != 0) {
                if (revision != scriptdata.Revision)
                    scriptdata = await archive.GetArchivedObject<Script>(id, revision??0);
            }

            script = await Parse(scriptdata);
            cache.StoreObject(id, revision??0, script);
            return script;
        }

        /// <inheritdoc />
        public async Task<CompiledScript> CompileScript(string name, int? revision = null) {
            CompiledScript script = cache.GetObject<CompiledScript, string>(name, revision??0);
            if(script != null)
                return script;

            Script scriptdata = await scriptservice.GetScript(name);
            if(revision != 0) {
                if(revision != scriptdata.Revision)
                    scriptdata = await archive.GetArchivedObject<Script>(scriptdata.Id, revision??0);
            }

            script = await Parse(scriptdata);
            cache.StoreObject(script.Name, revision??0, scriptdata);
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