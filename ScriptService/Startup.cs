using System;
using System.Reflection;
using mamgo.services.Extensions;
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
using ScriptService.Services.Scripts;

namespace ScriptService {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            
            services.AddControllers();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IScriptParser, ScriptParser>();
            services.AddSingleton<IScriptCompiler, ScriptCompiler>();
            services.AddSingleton<IScriptService, DatabaseScriptService>();
            services.AddSingleton<IScriptExecutionService, ScriptExecutionService>();
            services.AddSingleton<IWorkflowService, DatabaseWorkflowService>();
            services.AddSingleton<IWorkflowExecutionService, WorkflowExecutionService>();
            services.AddSingleton<IArchiveService, DatabaseArchiveService>();
            services.AddSingleton<ITaskService, DatabaseTaskService>();
            services.AddSingleton<IImportProvider, MethodProviderService>();
            services.AddSingleton(s => ConnectDatabase());
            services.AddErrorHandlers();
            services.AddSingleton<IConfigureOptions<MvcOptions>, MvcConfiguration>();

            ILogger logger=LoggerFactory.Create(builder => {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                builder.AddEventSourceLogger();
            }).CreateLogger("Startup");

            IConfigurationSection assemblysection = Configuration.GetSection("Assemblies");
            if (assemblysection != null) {
                foreach (IConfigurationSection assembly in assemblysection.GetChildren()) {
                    logger.LogInformation($"Loading assembly '{assembly.Value}'");
                    try {
                        Assembly.LoadFile(assembly.Value);
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
                    Type servicetype = !string.IsNullOrEmpty(servicename) ? Type.GetType(servicename) : null;

                    string implementationname = service["Implementation"];
                    Type implementationtype = !string.IsNullOrEmpty(implementationname) ? Type.GetType(implementationname) : null;
                    if (servicetype == null)
                        servicetype = implementationtype;

                    if (servicetype == null) {
                        logger.LogWarning($"Unable to setup service '{service.Key}' using '{implementationname}'->'{servicename}'");
                        continue;
                    }

                    services.AddSingleton(servicetype, implementationtype);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
