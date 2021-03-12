using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Scripts;
using ScriptService.Dto.Tasks;
using ScriptService.Extensions;
using ScriptService.Services.Scripts;
using ScriptService.Services.Workflows;
using TaskStatus = ScriptService.Dto.TaskStatus;

namespace ScriptService.Services {

    /// <inheritdoc />
    public class ScriptExecutionService : IScriptExecutionService {
        readonly ILogger<ScriptExecutionService> logger;
        readonly ITaskService scriptinstances;
        readonly IScriptCompiler scriptcompiler;

        /// <summary>
        /// creates a new <see cref="ScriptExecutionService"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="scriptinstances">access to script instances</param>
        /// <param name="scriptcompiler">compiles code to executable scripts</param>
        public ScriptExecutionService(ILogger<ScriptExecutionService> logger, ITaskService scriptinstances, IScriptCompiler scriptcompiler) {
            this.logger = logger;
            this.scriptinstances = scriptinstances;
            this.scriptcompiler = scriptcompiler;
        }

        /// <inheritdoc />
        public Task<object> Execute(IScript script, WorkableLogger scriptlogger, IDictionary<string, object> variables, CancellationToken token) {
            IVariableProvider scopevariables = new StateVariableProvider(variables, new Variable("log", scriptlogger));
            return script.ExecuteAsync(scopevariables, token);
        }

        async Task<WorkableTask> Execute(IScript scriptinstance, WorkableTask scripttask, IDictionary<string, object> variables, TimeSpan? wait) {
            WorkableLogger scriptlogger = new WorkableLogger(logger, scripttask);
            scripttask.Task = Execute(scriptinstance, scriptlogger, variables, scripttask.Token.Token).ContinueWith(t => {
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
        public async Task<WorkableTask> Execute(long id, int? revision=null, IDictionary<string, object> variables = null, TimeSpan? wait=null) {
            CompiledScript script = await scriptcompiler.CompileScriptAsync(id, revision);
            IDictionary<string, object> runtimevariables = await variables.TranslateVariables(scriptcompiler, ScriptLanguage.NCScript);
            WorkableTask scripttask = scriptinstances.CreateTask(WorkableType.Script, script.Id, script.Revision, script.Name, runtimevariables);
            try {
                return await Execute(script.Instance, scripttask, runtimevariables, wait);
            }
            catch (Exception e) {
                scripttask.Log.Add(e.ToString());
                scripttask.Status = TaskStatus.Failure;
                await scriptinstances.FinishTask(scripttask.Id);
            }

            return scripttask;
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(string name, int? revision=null, IDictionary<string, object> variables=null, TimeSpan? wait = null) {
            CompiledScript script = await scriptcompiler.CompileScriptAsync(name, revision);
            IDictionary<string, object> runtimevariables = await variables.TranslateVariables(scriptcompiler, ScriptLanguage.NCScript);
            WorkableTask scripttask = scriptinstances.CreateTask(WorkableType.Script, script.Id, script.Revision, script.Name, runtimevariables);
            try {
                return await Execute(script.Instance, scripttask, runtimevariables, wait);
            }
            catch (Exception e) {
                scripttask.Log.Add(e.ToString());
                scripttask.Status = TaskStatus.Failure;
                await scriptinstances.FinishTask(scripttask.Id);
            }

            return scripttask;
        }

        /// <inheritdoc />
        public async Task<WorkableTask> Execute(NamedCode code, IDictionary<string, object> variables = null, TimeSpan? wait = null) {
            IDictionary<string, object> runtimevariables = await variables.TranslateVariables(scriptcompiler, ScriptLanguage.NCScript);
            WorkableTask scripttask = scriptinstances.CreateTask(WorkableType.Script, 0, 0, code.Name, runtimevariables);
            try {
                return await Execute(await scriptcompiler.CompileCodeAsync(code.Code, code.Language), scripttask, runtimevariables, wait);
            }
            catch (Exception e) {
                scripttask.Log.Add(e.ToString());
                scripttask.Status = TaskStatus.Failure;
                await scriptinstances.FinishTask(scripttask.Id);
            }

            return scripttask;
        }
    }
}