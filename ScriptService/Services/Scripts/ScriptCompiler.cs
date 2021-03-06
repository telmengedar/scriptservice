﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Scripts;
using ScriptService.Services.Cache;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Lua;
using ScriptService.Services.Python;
using ScriptService.Services.Scripts.Extensions;

namespace ScriptService.Services.Scripts {

    /// <inheritdoc />
    public class ScriptCompiler : IScriptCompiler {
        readonly ILogger<ScriptCompiler> logger;
        readonly IScriptService scriptservice;
        readonly IScriptParser parser;
        readonly ICacheService cache;
        readonly IArchiveService archive;
        readonly IScriptImportService importservice;
        readonly IPythonService pythonservice;
        readonly ILuaService luaservice;

        /// <summary>
        /// creates a new <see cref="ScriptCompiler"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="parser">parser used to parse scripts</param>
        /// <param name="cache">access to object cache</param>
        /// <param name="methodprovider">provides managed method hosts to scripts</param>
        /// <param name="scriptservice">used to load scripts if not found in cache</param>
        /// <param name="archive">archive used to load revisions</param>
        /// <param name="importservice">access to javascript imports</param>
        /// <param name="pythonservice">access to python script logic</param>
        /// <param name="luaservice">used to execute lua code</param>
        public ScriptCompiler(ILogger<ScriptCompiler> logger, IScriptParser parser, ICacheService cache, IMethodProviderService methodprovider, IScriptService scriptservice, IArchiveService archive, IScriptImportService importservice, IPythonService pythonservice, ILuaService luaservice) {
            this.parser = parser;
            this.cache = cache;
            this.scriptservice = scriptservice;
            this.archive = archive;
            this.importservice = importservice;
            this.pythonservice = pythonservice;
            this.luaservice = luaservice;
            this.logger = logger;

            if (parser != null) {
                parser.Extensions.AddExtensions(typeof(Math));
                parser.Extensions.AddExtensions<ScriptEnumerations>();
                parser.ImportProvider = methodprovider;
            }

            ReactInitializer.Initialize();
        }



        async Task<CompiledScript> Parse(Script scriptdata) {
            logger.LogInformation($"Parsing script '{scriptdata.Id}.{scriptdata.Revision}'");
            IScript script = await CompileCodeAsync(scriptdata.Code, scriptdata.Language);

            return new CompiledScript {
                Id = scriptdata.Id,
                Revision = scriptdata.Revision,
                Name = scriptdata.Name,
                Instance = script
            };
        }

        /// <inheritdoc />
        public async Task<CompiledScript> CompileScriptAsync(long id, int? revision=null) {
            CompiledScript script = cache.GetObject<CompiledScript, long>(id, revision??0);
            if (script != null)
                return script;

            Script scriptdata = await scriptservice.GetScript(id);
            if (revision.HasValue && revision.Value != 0) {
                if (revision != scriptdata.Revision)
                    scriptdata = await archive.GetArchivedObject<Script>(id, revision.Value);
            }

            script = await Parse(scriptdata);
            cache.StoreObject(id, revision??0, script);
            return script;
        }

        /// <inheritdoc />
        public async Task<CompiledScript> CompileScriptAsync(string name, int? revision = null) {
            CompiledScript script = cache.GetObject<CompiledScript, string>(name, revision??0);
            if(script != null)
                return script;

            Script scriptdata = await scriptservice.GetScript(name);
            if(revision.HasValue && revision.Value != 0) {
                if(revision != scriptdata.Revision)
                    scriptdata = await archive.GetArchivedObject<Script>(scriptdata.Id, revision.Value);
            }

            script = await Parse(scriptdata);
            cache.StoreObject(script.Name, revision??0, scriptdata);
            return script;
        }

        /// <inheritdoc />
        public IScript CompileCode(string code, ScriptLanguage language) {
            if (string.IsNullOrEmpty(code))
                return null;

            switch(language) {
            case ScriptLanguage.NCScript:
                return parser.Parse(code);
            case ScriptLanguage.JavaScript:
            case ScriptLanguage.TypeScript:
                return new Dto.Scripts.JavaScript(code, importservice, language);
            default:
                throw new ArgumentException($"Unsupported script language '{language}'");
            }
        }

        /// <inheritdoc />
        public async Task<IScript> CompileCodeAsync(string code, ScriptLanguage language) {
            if (string.IsNullOrEmpty(code))
                return null;

            switch (language) {
            case ScriptLanguage.NCScript:
                return await parser.ParseAsync(code);
            case ScriptLanguage.JavaScript:
            case ScriptLanguage.TypeScript:
                return new Dto.Scripts.JavaScript(code, importservice, language);
            case ScriptLanguage.Python:
                return new PythonScript(pythonservice, code);
            case ScriptLanguage.Lua:
                return new LuaScript(code, luaservice);
            default:
                throw new ArgumentException($"Unsupported script language '{language}'");
            }
        }
    }
}