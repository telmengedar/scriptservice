using System.Threading.Tasks;
using NightlyCode.Scripting;
using ScriptService.Dto.Scripts;

namespace ScriptService.Services.Scripts {

    /// <summary>
    /// compiles code to executable script
    /// </summary>
    public interface IScriptCompiler {

        /// <summary>
        /// compiles a code to an executable script based on an id and revision
        /// </summary>
        /// <remarks>
        /// this method is meant to implement caching
        /// </remarks>
        /// <param name="id">id of script</param>
        /// <param name="revision">script revision</param>
        /// <returns>compiled script</returns>
        Task<CompiledScript> CompileScript(long id, int? revision=null);

        /// <summary>
        /// compiles a code to an executable script based on an id and revision
        /// </summary>
        /// <remarks>
        /// this method is meant to implement caching
        /// </remarks>
        /// <param name="name">name of script</param>
        /// <param name="revision">script revision</param>
        /// <returns>compiled script</returns>
        Task<CompiledScript> CompileScript(string name, int? revision = null);


        /// <summary>
        /// compiles a code to an executable script based on an id and revision
        /// </summary>
        /// <param name="code">code to compile</param>
        /// <returns>compiled script</returns>
        Task<IScript> CompileCode(string code);
    }
}