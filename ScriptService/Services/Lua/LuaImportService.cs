using System;
using Microsoft.Extensions.DependencyInjection;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.Lua {

    /// <summary>
    /// provides imports for lua scripts
    /// </summary>
    public class LuaImportService {
        readonly IServiceProvider serviceprovider;
        readonly WorkableLogger logger;

        /// <summary>
        /// creates a new <see cref="ScriptImportService"/>
        /// </summary>
        /// <param name="serviceprovider">used for service access</param>
        /// <param name="logger">logger used for script and workflow logging</param>
        public LuaImportService(IServiceProvider serviceprovider, WorkableLogger logger) {
            this.serviceprovider = serviceprovider;
            this.logger = logger;
        }

        /// <summary>
        /// loads a host method provider
        /// </summary>
        /// <param name="name">name of host to import</param>
        /// <returns>host value</returns>
        public object Host(string name) {
            return serviceprovider.GetService<IMethodProviderService>().GetHost(name);
        }

        /// <summary>
        /// loads a script
        /// </summary>
        /// <param name="name">name of script to load</param>
        /// <param name="revision">script revision to load</param>
        /// <returns>executor used to execute script</returns>
        public IWorkableExecutor Script(string name, int? revision) {
            return new LuaScriptExecutor(logger, serviceprovider.GetService<IScriptCompiler>(), name, revision);
        }

        /// <summary>
        /// loads a workflow
        /// </summary>
        /// <param name="name">name of workflow to load</param>
        /// <param name="revision">workflow revision to load</param>
        /// <returns>executor used to execute workflow</returns>
        public IWorkableExecutor Workflow(string name, int? revision) {
            return new LuaWorkableExecutor(logger, serviceprovider.GetService<IWorkflowService>(), serviceprovider.GetService<IWorkflowCompiler>(), serviceprovider.GetService<IWorkflowExecutionService>(), name, revision);
        }
    }
}