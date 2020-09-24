using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using NightlyCode.Scripting.Tokens;

namespace ScriptService.Services.Scripts {

    /// <summary>
    /// fake script providing a constant value
    /// </summary>
    public class ConstantValueScript : IScript {
        readonly object value;

        /// <summary>
        /// creates a new <see cref="ConstantValueScript"/>
        /// </summary>
        /// <param name="value">value to provide</param>
        public ConstantValueScript(object value) {
            this.value = value;
        }

        /// <inheritdoc />
        public object Execute(IDictionary<string, object> variables) {
            return value;
        }

        /// <inheritdoc />
        public object Execute(IVariableProvider variables = null) {
            return value;
        }

        /// <inheritdoc />
        public T Execute<T>(IDictionary<string, object> variables) {
            return Converter.Convert<T>(value);
        }

        /// <inheritdoc />
        public T Execute<T>(IVariableProvider variables = null) {
            return Converter.Convert<T>(value);
        }

        /// <inheritdoc />
        public Task<object> ExecuteAsync(IDictionary<string, object> variables, CancellationToken cancellationtoken = new CancellationToken()) {
            return Task.FromResult(value);
        }

        /// <inheritdoc />
        public Task<object> ExecuteAsync(IVariableProvider variables = null, CancellationToken cancellationtoken = new CancellationToken()) {
            return Task.FromResult(value);
        }

        /// <inheritdoc />
        public Task<T> ExecuteAsync<T>(IDictionary<string, object> variables, CancellationToken cancellationtoken = new CancellationToken()) {
            return Task.FromResult(Converter.Convert<T>(value));
        }

        /// <inheritdoc />
        public Task<T> ExecuteAsync<T>(IVariableProvider variables = null, CancellationToken cancellationtoken = new CancellationToken()) {
            return Task.FromResult(Converter.Convert<T>(value));
        }

        /// <inheritdoc />
        public IScriptToken Body => new ScriptValue(value);
    }
}