using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Scripting.Data;
using ScriptService.Services.Providers;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;

namespace ScriptService.Services {

    /// <summary>
    /// service used to provide methods to scripts and workflows
    /// </summary>
    public class MethodProviderService : IMethodProviderService {
        readonly IServiceProvider serviceprovider;
        IWorkflowExecutionService workflowexecutor;
        IWorkflowService workflowservice;
        IScriptCompiler compiler;
        IWorkflowCompiler workflowcompiler;
        readonly Dictionary<string, object> hosts=new Dictionary<string, object>();

        /// <summary>
        /// creates a new <see cref="MethodProviderService"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="serviceprovider">used to provide script and workflow executor on first request (to prevent cyclic dependencies)</param>
        /// <param name="configuration">configuration containing hosts to load</param>
        public MethodProviderService(ILogger<MethodProviderService> logger, IServiceProvider serviceprovider, IConfiguration configuration) {
            this.serviceprovider = serviceprovider;
            IConfigurationSection servicesection = configuration.GetSection("Services");
            if (servicesection != null) {
                logger.LogInformation("Loading service hosts from configuration");
                foreach (IConfigurationSection service in servicesection.GetChildren()) {
                    logger.LogInformation($"Loading service '{service.Key}'");
                    Type hosttype = Type.GetType(service["Service"] ?? service["Implementation"]);

                    if (hosttype == null) {
                        logger.LogWarning($"Type '{service["Service"] ?? service["Implementation"]}' not found.");
                        continue;
                    }

                    try {
                        object host = serviceprovider.GetService(hosttype);
                        hosts[service.Key.ToLower()] = host;
                    }
                    catch (Exception e) {
                        logger.LogError(e, $"Error loading host '{service.Key}'");
                    }
                }
            }
            else logger.LogInformation("No method hosts found in configuration");
        }

        /// <summary>
        /// hosts available to services
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Hosts => hosts;

        /// <inheritdoc />
        public object GetHost(string hostname) {
            if (!hosts.TryGetValue(hostname, out object hostinstance))
                throw new NotFoundException(typeof(object), hostname);
            return hostinstance;
        }

        /// <inheritdoc />
        public object Import(object[] parameters) {
            string source = parameters.FirstOrDefault()?.ToString();
            switch (source?.ToLower()) {
            case "host":
                return ProvideHostObject(parameters);
            case "script":
                return ProvideScript(parameters);
            case "workflow":
                return ProvideWorkflow(parameters);
            default:
                throw new ArgumentException($"Method host '{source}' not supported");
            }
        }

        IScriptCompiler Compiler {
            get {
                compiler ??= serviceprovider.GetService<IScriptCompiler>();
                return compiler;
            }
        }

        IWorkflowCompiler WorkflowCompiler {
            get {
                return workflowcompiler ??= serviceprovider.GetService<IWorkflowCompiler>();
            }
        }

        IWorkflowExecutionService WorkflowExecutor {
            get {
                workflowexecutor ??= serviceprovider.GetService<IWorkflowExecutionService>();
                return workflowexecutor;
            }
        }

        IWorkflowService WorkflowService {
            get {
                return workflowservice ??= serviceprovider.GetService<IWorkflowService>();
            }
        }

        IExternalMethod ProvideWorkflow(object[] parameters) {
            if(parameters.Length < 2)
                throw new ArgumentException("Name or id of workflow to import is required");

            int? revision = null;
            if (parameters.Length > 2 && parameters[2] is int revisionargument)
                revision = revisionargument;

            if(parameters[1] is long workflowid)
                return new WorkflowIdMethod(workflowid, revision, WorkflowService, WorkflowExecutor, WorkflowCompiler);

            if(parameters[1] is string workflowname)
                return new WorkflowNameMethod(workflowname, revision, WorkflowService, WorkflowExecutor, WorkflowCompiler);

            throw new ArgumentException($"Invalid workflow id/name '{parameters[1]}'");

        }

        IExternalMethod ProvideScript(object[] parameters) {
            if (parameters.Length < 2)
                throw new ArgumentException("Name or id of script to import is required");

            if (parameters[0] is long scriptid)
                return new ScriptIdMethod(scriptid, Compiler);

            if (parameters[1] is string scriptname)
                return new ScriptNameMethod(scriptname, Compiler);

            throw new ArgumentException($"Invalid script id/name '{parameters[1]}'");
        }

        object ProvideHostObject(object[] parameters) {
            if(parameters.Length < 2)
                throw new ArgumentException("Name of host to import is required");

            string hostname = parameters[1]?.ToString();
            if(string.IsNullOrEmpty(hostname))
                throw new ArgumentException("Name of host can't be empty");

            if(!hosts.TryGetValue(hostname.ToLower(), out object host))
                throw new ArgumentException($"Unknown host '{hostname}'");

            return host;
        }
    }
}