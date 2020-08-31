using System.Collections.Generic;
using NightlyCode.Scripting.Providers;

namespace ScriptService.Services {
    /// <summary>
    /// service which provides hosts to scripts and workflows
    /// </summary>
    public interface IMethodProviderService : IImportProvider {

        /// <summary>
        /// hosts available to services
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Hosts { get; }

        /// <summary>
        /// get installed host
        /// </summary>
        /// <param name="hostname">name of host to get</param>
        /// <returns>host object</returns>
        object GetHost(string hostname);
    }
}