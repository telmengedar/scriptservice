using System.Collections.Generic;
using System.Linq;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Errors;
using NightlyCode.Scripting.Parser;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// provides state variables to expressions
    /// </summary>
    public class StateVariableProvider : IVariableProvider {
        readonly IVariableProvider parentprovider;

        /// <summary>
        /// creates a new <see cref="VariableProvider"/>
        /// </summary>
        /// <param name="variables">variables to provide</param>
        /// <param name="additional">additional variables to add to variable pool</param>
        public StateVariableProvider(IDictionary<string, object> variables, params Variable[] additional) {
            Values = variables ?? new Dictionary<string, object>();
            foreach (Variable variable in additional)
                Values[variable.Name] = variable.Value;
        }

        /// <summary>
        /// creates a new <see cref="VariableProvider"/>
        /// </summary>
        /// <param name="parentprovider">parent variable scope</param>
        /// <param name="variables">variables to provide</param>
        public StateVariableProvider(IVariableProvider parentprovider, IDictionary<string, object> variables) {
            this.parentprovider = parentprovider;
            Values = variables;
        }

        /// <inheritdoc />
        public object this[string name] {
            get => GetVariable(name);
            set => Values[name] = value;
        }

        /// <summary>
        /// access to value lookup
        /// </summary>
        protected IDictionary<string, object> Values { get; }

        /// <inheritdoc />
        public object GetVariable(string name) {
            if(!ContainsVariable(name))
                throw new ScriptRuntimeException($"Variable {name} not found", null);

            return Values[name];
        }

        /// <inheritdoc />
        public bool ContainsVariable(string name) {
            return Values.ContainsKey(name);
        }

        /// <inheritdoc />
        public bool ContainsVariableInHierarchy(string name) {
            return Values.ContainsKey(name) || (parentprovider?.ContainsVariable(name) ?? false);
        }

        /// <summary>
        /// get provider in chain which contains a variable with the specified name
        /// </summary>
        /// <param name="variable">name of variable to check for</param>
        /// <returns>this if this provider contains this variable, null otherwise</returns>
        public IVariableProvider GetProvider(string variable) {
            if(ContainsVariable(variable))
                return this;
            return parentprovider?.GetProvider(variable);
        }

        /// <inheritdoc />
        public IEnumerable<string> Variables {
            get {
                if(parentprovider != null)
                    return Values.Keys.Concat(parentprovider.Variables);
                return Values.Keys;
            }
        }
    }
}