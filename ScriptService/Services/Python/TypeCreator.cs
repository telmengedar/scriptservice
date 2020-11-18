using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NightlyCode.Scripting.Extensions;

namespace ScriptService.Services.Python {
    
    /// <inheritdoc />
    public class TypeCreator : ITypeCreator {
        readonly Dictionary<string, Type> types = new Dictionary<string, Type>();

        /// <summary>
        /// creates a new <see cref="TypeCreator"/>
        /// </summary>
        /// <param name="logger">used to log configuration errors</param>
        /// <param name="configuration">configuration where type info is stored</param>
        public TypeCreator(ILogger<TypeCreator> logger, IConfiguration configuration) {
            IConfigurationSection typesection = configuration.GetSection("Types");
            if (typesection != null) {
                foreach (IConfigurationSection type in typesection.GetChildren()) {
                    Type typedef = Type.GetType(type.Value);
                    if (typedef == null) {
                        logger.LogWarning($"Unable to find type '{type.Value}'");
                        continue;
                    }

                    logger.LogInformation($"Adding '{typedef}' as '{type.Key}'");
                    types.Add(type.Key, typedef);
                }
            }
        }

        /// <inheritdoc />
        public Type this[string key] => types[key];

        /// <inheritdoc />
        public bool Contains(string type) {
            return types.ContainsKey(type);
        }

        /// <inheritdoc />
        public object New(string typename) {
            if (!types.TryGetValue(typename, out Type type))
                throw new ArgumentException($"unknown type '{typename}'", nameof(typename));
            return Activator.CreateInstance(type);
        }

        /// <inheritdoc />
        public object New(string typename, object[] parameters) {
            if (!types.TryGetValue(typename, out Type type))
                throw new ArgumentException($"unknown type '{typename}'", nameof(typename));
            return Activator.CreateInstance(type, parameters);
        }

        /// <inheritdoc />
        public void Init(object instance, IDictionary<string, object> arguments) {
            arguments.FillType(instance);
        }
    }
}