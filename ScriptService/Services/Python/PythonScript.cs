using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Tokens;

namespace ScriptService.Services.Python {
    
    /// <summary>
    /// python script
    /// </summary>
    public class PythonScript : IScript {
        readonly IPythonService pythonservice;
        readonly ScriptSource script;
        
        /// <summary>
        /// creates a new <see cref="PythonScript"/>
        /// </summary>
        /// <param name="pythonservice">service used to interpret and execute script</param>
        /// <param name="code">python code to execute</param>
        public PythonScript(IPythonService pythonservice, string code) {
            this.pythonservice = pythonservice;
            script = pythonservice.Parse(code);
        }

        /// <inheritdoc />
        public object Execute(IDictionary<string, object> variables) {
            return pythonservice.Execute(script, variables);
        }

        /// <inheritdoc />
        public object Execute(IVariableProvider variables = null) {
            Dictionary<string,object> dic=new Dictionary<string, object>();
            if (variables != null) {
                foreach (string key in variables.Variables)
                    dic[key] = variables.GetProvider(key).GetVariable(key);
            }

            return Execute(dic);
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
            return Task.Run(() => Execute(variables), cancellationtoken);
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

        /// <inheritdoc />
        public IScriptToken Body => null;
    }
}