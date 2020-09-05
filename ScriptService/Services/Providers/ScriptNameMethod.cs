using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Data;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Tasks;
using ScriptService.Errors;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Providers {

    /// <summary>
    /// method which executes a script by name
    /// </summary>
    public class ScriptNameMethod : ScriptMethod {
        readonly string scriptname;
        IScriptCompiler compiler;

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
            return (await compiler.CompileScript(scriptname)).Instance;
        }
    }
}