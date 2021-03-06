﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptService.Dto.Workflows;

namespace ScriptService.Services.Workflows {

    /// <summary>
    /// compiled workflow data to executable instances
    /// </summary>
    public interface IWorkflowCompiler {

        /// <summary>
        /// builds an executable workflow from workflow details
        /// </summary>
        /// <param name="workflow">workflow to instantiate</param>
        /// <returns>instantiated workflow</returns>
        Task<WorkflowInstance> BuildWorkflow(WorkflowDetails workflow);

        /// <summary>
        /// builds an executable workflow from workflow details
        /// </summary>
        /// <param name="workflowid">id of workflow to build</param>
        /// <param name="revision">revision of workflow to build (optional)</param>
        /// <returns>executable workflow instance</returns>
        Task<WorkflowInstance> BuildWorkflow(long workflowid, int? revision=null);

        /// <summary>
        /// builds an executable workflow from workflow details
        /// </summary>
        /// <param name="workflowname">name of workflow to build</param>
        /// <param name="revision">revision of workflow to build (optional)</param>
        /// <returns>executable workflow instance</returns>
        Task<WorkflowInstance> BuildWorkflow(string workflowname, int? revision = null);

        /// <summary>
        /// build a <see cref="WorkflowInstance"/> from a <see cref="WorkflowStructure"/>
        /// </summary>
        /// <param name="workflow">structure to compile to an executable workflow</param>
        /// <returns>executable workflow instance</returns>
        Task<WorkflowInstance> BuildWorkflow(WorkflowStructure workflow);
    }
}