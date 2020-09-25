using System.Collections.Generic;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.JavaScript {
    /// <summary>
    /// executes scripts from scripts
    /// </summary>
    public class ScriptExecutor : IWorkableExecutor {
        readonly WorkableLogger logger;
        readonly IScriptCompiler compiler;
        readonly string name;
        readonly int? revision;

        /// <summary>
        /// creates a new <see cref="ScriptExecutor"/>
        /// </summary>
        /// <param name="logger">used for logging in script</param>
        /// <param name="compiler">compiler used to parse script</param>
        /// <param name="name">name of script to execute</param>
        /// <param name="revision">revision of script to execute</param>
        public ScriptExecutor(WorkableLogger logger, IScriptCompiler compiler, string name, int? revision) {
            this.logger = logger;
            this.compiler = compiler;
            this.name = name;
            this.revision = revision;
        }

        /// <inheritdoc />
        public object Execute(IDictionary<string, object> arguments) {
            arguments["log"] = logger;
            return compiler.CompileScriptAsync(name, revision).GetAwaiter().GetResult().Instance.Execute(arguments);
        }
    }
}