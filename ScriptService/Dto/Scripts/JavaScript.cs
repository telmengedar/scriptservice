using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jint;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Tokens;
using ScriptService.Services;
using ScriptService.Services.JavaScript;

namespace ScriptService.Dto.Scripts {

    /// <summary>
    /// script executing javascript
    /// </summary>
    public class JavaScript : IScript {
        readonly Esprima.Ast.Script script;
        readonly IJavascriptImportService importservice;

        /// <summary>
        /// creates a new <see cref="JavaScript"/>
        /// </summary>
        /// <param name="script">script to execute</param>
        /// <param name="importservice">access to imports in javascript</param>
        public JavaScript(Esprima.Ast.Script script, IJavascriptImportService importservice) {
            this.script = script;
            this.importservice = importservice;
        }

        /// <inheritdoc />
        public object Execute(IDictionary<string, object> variables) {
            return Execute(new VariableProvider(variables));
        }

        /// <inheritdoc />
        public object Execute(IVariableProvider variables) {
            if (variables == null)
                throw new InvalidOperationException("Javascript execution needs global variables");

            WorkableLogger logger=variables.GetProvider("log").GetVariable("log") as WorkableLogger;

            Engine engine = new Engine {
                ClrTypeConverter = new JavascriptTypeConverter()
            };
            engine.SetValue("load", importservice.Clone(logger));
            
            foreach (string name in variables.Variables) {
                
                engine.SetValue(name, variables[name]);
            }

            return engine.Execute(script).GetCompletionValue().ToObject();
        }

        /// <inheritdoc />
        public T Execute<T>(IDictionary<string, object> variables) {
            return Converter.Convert<T>(Execute(variables));
        }

        /// <inheritdoc />
        public T Execute<T>(IVariableProvider variables = null) {
            return Converter.Convert<T>(Execute(variables));
        }

        /// <inheritdoc />
        public Task<object> ExecuteAsync(IDictionary<string, object> variables, CancellationToken cancellationtoken = new CancellationToken()) {
            return ExecuteAsync(new VariableProvider(variables), cancellationtoken);
        }

        /// <inheritdoc />
        public Task<object> ExecuteAsync(IVariableProvider variables = null, CancellationToken cancellationtoken = new CancellationToken()) {
            return Task.Run(() => Execute(variables), cancellationtoken);
        }

        /// <inheritdoc />
        public async Task<T> ExecuteAsync<T>(IDictionary<string, object> variables, CancellationToken cancellationtoken = new CancellationToken()) {
            return Converter.Convert<T>(await ExecuteAsync(variables, cancellationtoken));
        }

        /// <inheritdoc />
        public async Task<T> ExecuteAsync<T>(IVariableProvider variables = null, CancellationToken cancellationtoken = new CancellationToken()) {
            return Converter.Convert<T>(await ExecuteAsync(variables, cancellationtoken));
        }

        /// <summary>
        /// not supported
        /// </summary>
        public IScriptToken Body => null;
    }
}