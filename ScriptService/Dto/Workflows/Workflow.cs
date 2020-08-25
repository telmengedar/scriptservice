using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto.Workflows {

    /// <summary>
    /// Workflow definition
    /// </summary>
    public class Workflow : WorkflowData {

        /// <summary>
        /// id of workflow
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// Workflow revision
        /// </summary>
        public int Revision { get; set; }
    }
}