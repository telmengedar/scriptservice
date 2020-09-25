using System.Collections.Generic;

namespace ScriptService.Services.JavaScript {

    /// <summary>
    /// executes a workable
    /// </summary>
    public interface IWorkableExecutor {

        /// <summary>
        /// executes the workable
        /// </summary>
        /// <param name="arguments">arguments for execution</param>
        /// <returns>workable execution result</returns>
        object Execute(IDictionary<string, object> arguments);
    }
}