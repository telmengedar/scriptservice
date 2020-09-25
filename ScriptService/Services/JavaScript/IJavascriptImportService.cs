using System;

namespace ScriptService.Services.JavaScript {

    /// <summary>
    /// service used to provide imports to javascript
    /// </summary>
    public interface IJavascriptImportService {

        /// <summary>
        /// imports an object for usage in javascript
        /// </summary>
        /// <param name="name">name of host to import</param>
        /// <returns>host object</returns>
        object Host(string name);

        /// <summary>
        /// imports an object for usage in javascript
        /// </summary>
        /// <param name="name">name of script to import</param>
        /// <param name="revision">script revision</param>
        /// <returns>script executor</returns>
        IWorkableExecutor Script(string name, int? revision);

        /// <summary>
        /// imports an object for usage in javascript
        /// </summary>
        /// <param name="name">name of workflow to import</param>
        /// <param name="revision">script revision</param>
        /// <returns>workflow executor</returns>
        IWorkableExecutor Workflow(string name, int? revision);

        /// <summary>
        /// creates a new <see cref="IJavascriptImportService"/> with a new logger
        /// </summary>
        /// <param name="logger">logger to use</param>
        /// <returns>cloned import service</returns>
        IJavascriptImportService Clone(WorkableLogger logger);
    }
}