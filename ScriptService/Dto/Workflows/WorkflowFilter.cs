using NightlyCode.AspNetCore.Services.Data;

namespace ScriptService.Dto.Workflows {
    
    /// <summary>
    /// filter for workflow listing
    /// </summary>
    public class WorkflowFilter : ListFilter {
        
        /// <summary>
        /// workflow names to filter for
        /// </summary>
        public string[] Name { get; set; }
    }
}