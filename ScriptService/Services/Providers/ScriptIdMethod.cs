using System.Threading.Tasks;
using NightlyCode.Scripting;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// executes a script by id
    /// </summary>
    public class ScriptIdMethod : ScriptMethod {
        readonly long scriptid;
        readonly IScriptCompiler compiler;

        /// <summary>
        /// creates a new <see cref="ScriptIdMethod"/>
        /// </summary>
        /// <param name="scriptid">id of script</param>
        /// <param name="compiler">compiler used to retrieve script instance</param>
        public ScriptIdMethod(long scriptid, IScriptCompiler compiler) {
            this.scriptid = scriptid;
            this.compiler = compiler;
        }

        /// <inheritdoc />
        protected override async Task<IScript> LoadScript() {
            return (await compiler.CompileScript(scriptid)).Instance;
        }
    }
}