using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Esprima;
using Jint;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Tokens;
using React;
using ScriptService.Services;
using ScriptService.Services.JavaScript;

namespace ScriptService.Dto.Scripts {

    /// <summary>
    /// script executing javascript
    /// </summary>
    public class JavaScript : IScript {
        Esprima.Ast.Script script;
        readonly string code;
        readonly IScriptImportService importservice;
        readonly ScriptLanguage language;

        /// <summary>
        /// creates a new <see cref="JavaScript"/>
        /// </summary>
        /// <param name="code">code to execute</param>
        /// <param name="importservice">access to imports in javascript</param>
        /// <param name="language">language to evaluate (optional, must be a javascript relative)</param>
        public JavaScript(string code, IScriptImportService importservice, ScriptLanguage language=ScriptLanguage.JavaScript) {
            this.code = code;
            this.importservice = importservice;
            this.language = language;
        }

        /// <inheritdoc />
        public object Execute(IDictionary<string, object> variables) {
            return Execute(new VariableProvider(variables));
        }

        /// <inheritdoc />
        public object Execute(IVariableProvider variables) {
            if (variables == null)
                throw new InvalidOperationException("Javascript execution needs global variables");

            Engine engine = new Engine {
                ClrTypeConverter = new JavascriptTypeConverter()
            };
            
            WorkableLogger logger=variables.GetProvider("log").GetVariable("log") as WorkableLogger;
            engine.SetValue("load", importservice.Clone(logger));
            
            foreach (string name in variables.Variables) {
                object hostobject = variables[name];
                if (hostobject == null)
                    // v8 doesn't like null host objects
                    continue;
                engine.SetValue(name, hostobject);
            }

            switch (language) {
            case ScriptLanguage.JavaScript:
                script ??= new JavaScriptParser(code).ParseScript();
                return engine.Execute(script).GetCompletionValue().ToObject();
            case ScriptLanguage.TypeScript:
                string transpiled = ReactEnvironment.Current.Babel.Transform($"function main():any{{{code}}}\nvar _result=main();", "unknown.ts");
                script ??= new JavaScriptParser(transpiled).ParseScript();
                engine.Execute(script);
                return engine.GetValue("_result").ToObject();
            default:
                throw new ArgumentException($"Unsupported language '{language}' as JavaScript");
            }
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