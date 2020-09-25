using System.Threading.Tasks;
using NightlyCode.Scripting;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// method which executes a script by name
    /// </summary>
    public class ScriptNameMethod : ScriptMethod {
        readonly string scriptname;
        readonly IScriptCompiler compiler;

        /// <summary>
        /// creates a new <see cref="ScriptIdMethod"/>
        /// </summary>
        /// <param name="scriptname">name of script</param>
        /// <param name="compiler">compiler used to retrieve script instances</param>
        public ScriptNameMethod(string scriptname, IScriptCompiler compiler) {
            this.scriptname = scriptname;
            this.compiler = compiler;
        }

        /// <inheritdoc />
        protected override async Task<IScript> LoadScript() {
            return (await compiler.CompileScriptAsync(scriptname)).Instance;
        }
    }
}