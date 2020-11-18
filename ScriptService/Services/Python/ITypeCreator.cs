using System;
using System.Collections.Generic;

namespace ScriptService.Services.Python {
    
    /// <summary>
    /// creates types from names and arguments for script languages not supporting type references
    /// </summary>
    public interface ITypeCreator {

        /// <summary>
        /// indexer for type information
        /// </summary>
        /// <param name="key">name of type to get</param>
        Type this[string key] { get; }
            
        /// <summary>
        /// determines whether a type is known
        /// </summary>
        /// <param name="type">name of type to check for</param>
        /// <returns>true if type is known, false otherwise</returns>
        bool Contains(string type);
        
        
        /// <summary>
        /// creates a type object from name
        /// </summary>
        /// <param name="typename">name of type to create</param>
        /// <returns>created type</returns>
        object New(string typename);

        /// <summary>
        /// creates a type object from name and arguments
        /// </summary>
        /// <param name="typename">name of type to create</param>
        /// <param name="parameters">constructor parameters</param>
        /// <returns>created type</returns>
        object New(string typename, object[] parameters);

        /// <summary>
        /// initializes properties of an instance using a dictionary
        /// </summary>
        /// <param name="instance">object to initialize</param>
        /// <param name="arguments">arguments used to initialize type properties</param>
        void Init(object instance, IDictionary<string, object> arguments);
    }
}