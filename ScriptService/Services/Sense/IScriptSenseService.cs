using System.Threading.Tasks;
using ScriptService.Dto.Sense;

namespace ScriptService.Services.Sense {

    /// <summary>
    /// provides information about script environment
    /// </summary>
    public interface IScriptSenseService {

        /// <summary>
        /// get info about a script type
        /// </summary>
        /// <returns>type info</returns>
        Task<TypeInfo> GetTypeInfo(string typename);

        /// <summary>
        /// get installed type providers
        /// </summary>
        /// <returns>type providers available to scripts</returns>
        Task<PropertyInfo[]> GetTypeProviders();

        /// <summary>
        /// get information about installed host providers
        /// </summary>
        /// <returns>info about installed host providers</returns>
        Task<PropertyInfo[]> GetHostProviders();
    }
}