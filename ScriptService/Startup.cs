using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NightlyCode.AspNetCore.Services.Extensions;
using NightlyCode.AspNetCore.Services.Middleware;
using NightlyCode.Database.Clients;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Info;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Providers;
using Npgsql;
using ScriptService.Services;
using ScriptService.Services.Cache;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Lua;
using ScriptService.Services.Python;
using ScriptService.Services.Scripts;
using ScriptService.Services.Sense;
using ScriptService.Services.Tasks;
using ScriptService.Services.Workflows;

namespace ScriptService {

    /// <summary>
    /// startup for asp service initialization
    /// </summary>
    public class Startup {

        /// <summary>
        /// creates a new <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration">service configuration</param>
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }

        IEntityManager ConnectDatabase() {
            switch (Configuration["Database:Type"]) {
            case "Sqlite":
                return new EntityManager(ClientFactory.Create(new SqliteConnection($"Data Source={Configuration["Database:Source"]}"), new SQLiteInfo()));
            case "Postgres":
                return new EntityManager(ClientFactory.Create(() => new NpgsqlConnection($"Server={Configuration["Database:Host"]};Port={Configuration["Database:Port"]};Database={Configuration["Database:Database"]};User Id={Configuration["Database:User"]};Password={Configuration["Database:Password"]};"), new PostgreInfo(), true));
            default:
                throw new ArgumentException($"Database type {Configuration["Database:Type"]} not supported.");
            }
        }

        IScriptParser SetupScriptParser() {
            ILogger logger = LoggerFactory.Create(builder => {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                builder.AddEventSourceLogger();
            }).CreateLogger("Startup");

            ScriptParser parser =new ScriptParser();
            IConfigurationSection typesection = Configuration.GetSection("Types");
            if (typesection != null) {
                foreach (IConfigurationSection type in typesection.GetChildren()) {
                    Type typedef = Type.GetType(type.Value);
                    if (typedef == null) {
                        logger.LogWarning($"Unable to find type '{type.Value}'");
                        continue;
                    }

                    logger.LogInformation($"Adding '{typedef}' as '{type.Key}'");
                    parser.Types.AddType(type.Key, new TypeInstanceProvider(typedef, parser.MethodCallResolver));
                }
            }

            IConfigurationSection extensionssection = Configuration.GetSection("Extensions");
            string[] extensions = extensionssection?.Get<string[]>();
            if (extensions != null) {
                foreach (string extension in extensions) {
                    Type typedef = Type.GetType(extension);
                    if(typedef == null) {
                        logger.LogWarning($"Unable to find extension type '{extension}'");
                        continue;
                    }

                    logger.LogInformation($"Adding extension '{typedef}'");
                    parser.Extensions.AddExtensions(typedef);
                }
            }
            return parser;
        }

        /// <summary>
        /// configures services used by the script service
        /// </summary>
        /// <param name="services">access to service collection</param>
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton(s => SetupScriptParser());
            services.AddSingleton<IScriptCompiler, ScriptCompiler>();
            services.AddSingleton<IScriptService, DatabaseScriptService>();
            services.AddSingleton<IScriptExecutionService, ScriptExecutionService>();
            services.AddSingleton<IWorkflowService, DatabaseWorkflowService>();
            services.AddSingleton<IWorkflowExportService, WorkflowExportService>();
            services.AddSingleton<IWorkflowExecutionService, WorkflowExecutionService>();
            services.AddSingleton<IArchiveService, DatabaseArchiveService>();
            services.AddSingleton<ITaskService, DatabaseTaskService>();
            services.AddSingleton<IMethodProviderService, MethodProviderService>();
            services.AddSingleton<IScriptSenseService, ScriptSenseService>();
            services.AddSingleton<IScheduledTaskService, DatabaseScheduledTaskService>();
            services.AddSingleton<IWorkflowCompiler, WorkflowCompiler>();
            services.AddSingleton<IPythonService, PythonService>();
            services.AddSingleton<ILuaService, LuaService>();
            services.AddSingleton<IScriptImportService, ScriptImportService>();
            services.AddSingleton<ITypeCreator, TypeCreator>();
            
            services.AddHostedService<TaskScheduler>();
            services.AddSingleton(s => ConnectDatabase());
            services.AddErrorHandlers();
            services.AddSingleton<IConfigureOptions<MvcOptions>, MvcConfiguration>();

            ILogger logger=LoggerFactory.Create(builder => {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                builder.AddEventSourceLogger();
            }).CreateLogger("Startup");

            string contentRoot = Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            List<Assembly> assemblies=new List<Assembly>();
            IConfigurationSection assemblysection = Configuration.GetSection("Assemblies");
            if (assemblysection != null) {
                foreach (IConfigurationSection assembly in assemblysection.GetChildren()) {
                    logger.LogInformation($"Loading assembly '{assembly.Value}'");
                    try {
                        assemblies.Add(Assembly.LoadFrom(Path.Combine(contentRoot, assembly.Value)));
                    }
                    catch (Exception e) {
                        logger.LogError(e, $"Unable to load '{assembly.Value}'");
                    }
                }
            }

            IConfigurationSection servicesection = Configuration.GetSection("Services");
            if (servicesection != null) {
                foreach (IConfigurationSection service in servicesection.GetChildren()) {
                    string servicename = service["Service"];
                    Type servicetype=!string.IsNullOrEmpty(servicename) ? Type.GetType(servicename) : null;
                    if (servicetype == null && !string.IsNullOrEmpty(servicename))
                        servicetype = assemblies.Select(a => a.GetType(servicename)).FirstOrDefault(t => t != null);

                    string implementationname = service["Implementation"];
                    Type implementationtype = !string.IsNullOrEmpty(implementationname) ? Type.GetType(implementationname) : null;
                    if (implementationtype == null && !string.IsNullOrEmpty(implementationname))
                        implementationtype = assemblies.Select(a => a.GetType(implementationname)).FirstOrDefault(t => t != null);

                    if (servicetype == null)
                        servicetype = implementationtype;

                    if (servicetype == null) {
                        logger.LogWarning($"Unable to setup service '{service.Key}' using '{servicename}'->'{implementationname}'");
                        continue;
                    }

                    logger.LogInformation($"Adding service '{service.Key}' using '{servicename}'->'{implementationname}'");

                    services.AddSingleton(servicetype, implementationtype);
                }
            }
        }

        /// <summary>
        /// configures middleware of this service
        /// </summary>
        /// <param name="app">app to which to add middleware</param>
        /// <param name="env">runtime environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseRouting();
            app.UseDefaultCors();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
