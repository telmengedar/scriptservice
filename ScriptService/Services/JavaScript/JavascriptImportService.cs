﻿using System;
using Microsoft.Extensions.DependencyInjection;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;

namespace ScriptService.Services.JavaScript {

    /// <inheritdoc />
    public class JavascriptImportService : IJavascriptImportService {
        readonly IServiceProvider serviceprovider;
        readonly WorkableLogger logger;

        /// <summary>
        /// creates a new <see cref="JavascriptImportService"/>
        /// </summary>
        /// <param name="serviceprovider">used for service access</param>
        /// <param name="logger">logger used for script and workflow logging</param>
        public JavascriptImportService(IServiceProvider serviceprovider, WorkableLogger logger=null) {
            this.serviceprovider = serviceprovider;
            this.logger = logger;
        }

        /// <inheritdoc />
        public object Host(string name) {
            return serviceprovider.GetService<IMethodProviderService>().GetHost(name);
        }

        /// <inheritdoc />
        public IWorkableExecutor Script(string name, int? revision) {
            return new ScriptExecutor(logger, serviceprovider.GetService<IScriptCompiler>(), name, revision);
        }

        /// <inheritdoc />
        public IWorkableExecutor Workflow(string name, int? revision) {
            return new WorkableExecutor(logger, serviceprovider.GetService<IWorkflowService>(), serviceprovider.GetService<IWorkflowCompiler>(), serviceprovider.GetService<IWorkflowExecutionService>(), name, revision);
        }

        /// <inheritdoc />
        public IJavascriptImportService Clone(WorkableLogger logger) {
            return new JavascriptImportService(serviceprovider, logger);
        }
    }
}