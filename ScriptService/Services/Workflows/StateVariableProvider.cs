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
        /// adds arguments of a dictionary to the variable provider
        /// </summary>
        /// <param name="variables"></param>
        public void Add(IDictionary<string, object> variables) {
            foreach ((string key, object value) in variables)
                Values[key] = value;
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

        /// <summary>
        /// gets variable from provider or null if value was not found
        /// </summary>
        /// <param name="name">name of variable to get</param>
        /// <returns>value of variable or null if variable was not found</returns>
        public object GetOrDefault(string name) {
            Values.TryGetValue(name, out object value);
            return value;
        }

        /// <summary>
        /// tries to get value
        /// </summary>
        /// <param name="name">name of variable to get</param>
        /// <param name="value">value to store result in</param>
        /// <returns>true if variable was found, false otherwise</returns>
        public bool TryGetValue(string name, out object value) {
            return Values.TryGetValue(name, out value);
        }
        
        /// <inheritdoc />
        public bool ContainsVariable(string name) {
            return Values.ContainsKey(name);
        }

        /// <inheritdoc />
        public bool ContainsVariableInHierarchy(string name) {
            return ContainsVariable(name);
        }

        /// <summary>
        /// get provider in chain which contains a variable with the specified name
        /// </summary>
        /// <param name="variable">name of variable to check for</param>
        /// <returns>this if this provider contains this variable, null otherwise</returns>
        public IVariableProvider GetProvider(string variable) {
            if(ContainsVariable(variable))
                return this;
            return null;
        }

        /// <inheritdoc />
        public IEnumerable<string> Variables => Values.Keys;
    }
}