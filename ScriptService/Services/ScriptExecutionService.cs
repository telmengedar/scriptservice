using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Services.Scripts;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Services {

    /// <inheritdoc />
    public class ScriptExecutionService : IScriptExecutionService {
        readonly ILogger<ScriptExecutionService> logger;
        readonly IScriptService scriptservice;
        readonly ITaskService scriptinstances;
        readonly IScriptCompiler scriptcompiler;

        /// <summary>
        /// creates a new <see cref="ScriptExecutionService"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="scriptservice">service used to get script code to execute</param>
        /// <param name="scriptinstances">access to script instances</param>
        /// <param name="scriptcompiler">compiles code to executable scripts</param>
        public ScriptExecutionService(ILogger<ScriptExecutionService> logger, IScriptService scriptservice, ITaskService scriptinstances, IScriptCompiler scriptcompiler) {
            this.logger = logger;
            this.scriptservice = scriptservice;
            this.scriptinstances = scriptinstances;
            this.scriptcompiler = scriptcompiler;
        }

        async Task<WorkableTask> Execute(Script script, IDictionary<string, object> variables, TimeSpan? wait) {
            IScript scriptinstance = await scriptcompiler.CompileCode(script.Id, script.Revision, script.Code);

            WorkableTask scripttask = scriptinstances.CreateTask(WorkableType.Script, script.Id, script.Revision, script.Name, variables);
            WorkableLogger scriptlogger = new WorkableLogger(logger, scripttask);
            IVariableProvider scopevariables = new VariableProvider(new Variable("log", scriptlogger));
            IVariableProvider scriptvariables = variables != null ? new VariableProvider(scopevariables, variables) : scopevariables;
            scripttask.Task = scriptinstance.ExecuteAsync(scriptvariables, scripttask.Token.Token).ContinueWith(t => {
                if (t.IsCanceled) {
                    scriptlogger.Warning("Script execution was aborted");
                    scripttask.Status = TaskStatus.Canceled;
                }
                else if (t.IsFaulted) {
                    scriptlogger.Error("Script failed to execute", t.Exception?.InnerException ?? t.Exception);
                    scripttask.Status = TaskStatus.Failure;
                }
                else {
                    scripttask.Result = t.Result;
                    scriptlogger.Info($"Script executed successfully with result '{scripttask.Result}'");
                    scripttask.Status = TaskStatus.Success;
                }

                scripttask.Finished = DateTime.Now;
                scriptinstances.FinishTask(scripttask.Id).GetAwaiter().GetResult();
            });

            if (wait.HasValue && !scripttask.Task.IsCompleted)
                await Task.WhenAny(scripttask.Task, Task.Delay(wait.Value));

            return scripttask;
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(long id, IDictionary<string, object> variables = null, TimeSpan? wait=null) {
            Script script = await scriptservice.GetScript(id);
            return await Execute(script, variables, wait);
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(string name, IDictionary<string, object> variables=null, TimeSpan? wait = null) {
            Script script = await scriptservice.GetScript(name);
            return await Execute(script, variables, wait);
        }

        /// <inheritdoc />
        public Task<WorkableTask> Execute(NamedCode code, IDictionary<string, object> variables = null, TimeSpan? wait = null) {
            return Execute(new Script {
                Code = code.Code,
                Name = code.Name
            }, variables, wait);
        }
    }
}