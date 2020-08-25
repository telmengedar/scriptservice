using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Providers;
using ScriptService.Services.Providers;

namespace ScriptService.Services {

    /// <summary>
    /// service used to provide methods to scripts and workflows
    /// </summary>
    public class MethodProviderService : IImportProvider {
        readonly ILogger<MethodProviderService> logger;
        readonly IServiceProvider serviceprovider;
        IScriptExecutionService scriptexecutor;
        IWorkflowExecutionService workflowexecutor;
        readonly Dictionary<string, object> hosts=new Dictionary<string, object>();


        /// <summary>
        /// creates a new <see cref="MethodProviderService"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="serviceprovider">used to provide script and workflow executor on first request (to prevent cyclic dependencies)</param>
        /// <param name="configuration">configuration containing hosts to load</param>
        public MethodProviderService(ILogger<MethodProviderService> logger, IServiceProvider serviceprovider, IConfiguration configuration) {
            this.logger = logger;
            this.serviceprovider = serviceprovider;

            IConfigurationSection servicesection = configuration.GetSection("Hosts");
            if (servicesection != null) {
                logger.LogInformation("Loading method hosts from configuration");
                foreach (IConfigurationSection service in servicesection.GetChildren()) {
                    this.logger.LogInformation($"Loading host '{service.Key}' from '{service.Value}'");
                    try {
                        Type hosttype = Type.GetType(service.Value);
                        if (hosttype==null) {
                            logger.LogWarning($"Type '{service.Value}' not found");
                            continue;
                        }
                        object host = Activator.CreateInstance(hosttype);
                        hosts[service.Key.ToLower()] = host;
                    }
                    catch (Exception e) {
                        logger.LogError(e, $"Error loading host '{service.Key}'");
                    }
                }
            }
            else logger.LogInformation("No method hosts found in configuration");
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

        IScriptExecutionService ScriptExecutor {
            get {
                scriptexecutor ??= serviceprovider.GetService<IScriptExecutionService>();
                return scriptexecutor;
            }
        }

        IWorkflowExecutionService WorkflowExecutor {
            get {
                workflowexecutor ??= serviceprovider.GetService<IWorkflowExecutionService>();
                return workflowexecutor;
            }
        }

        IExternalMethod ProvideWorkflow(object[] parameters) {
            if(parameters.Length < 2)
                throw new ArgumentException("Name or id of workflow to import is required");

            if(parameters[0] is long workflowid)
                return new WorkflowIdMethod(workflowid, WorkflowExecutor);

            if(parameters[1] is string workflowname)
                return new WorkflowNameMethod(workflowname, WorkflowExecutor);

            throw new ArgumentException($"Invalid workflow id/name '{parameters[1]}'");

        }

        IExternalMethod ProvideScript(object[] parameters) {
            if (parameters.Length < 2)
                throw new ArgumentException("Name or id of script to import is required");

            if (parameters[0] is long scriptid)
                return new ScriptIdMethod(scriptid, ScriptExecutor);

            if (parameters[1] is string scriptname)
                return new ScriptNameMethod(scriptname, ScriptExecutor);

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