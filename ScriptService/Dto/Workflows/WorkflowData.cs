using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// data of a workflow
    /// </summary>
    public class WorkflowData {

        /// <summary>
        /// name of workflow
        /// </summary>
        [Unique]
        public string Name { get; set; }

        /// <summary>
        /// default script language for expressions
        /// </summary>
        public ScriptLanguage? Language { get; set; }
    }
}