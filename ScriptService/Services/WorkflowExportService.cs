using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using ScriptService.Dto.Workflows;

namespace ScriptService.Services {
    /// <inheritdoc />
    public class WorkflowExportService : IWorkflowExportService {
        readonly IWorkflowService workflowservice;

        /// <summary>
        /// creates a new <see cref="WorkflowExportService"/>
        /// </summary>
        /// <param name="workflowservice">service used to retrieve workflow data</param>
        public WorkflowExportService(IWorkflowService workflowservice) {
            this.workflowservice = workflowservice;
        }

        /// <inheritdoc />
        public async Task<WorkflowStructure> ExportWorkflow(long workflowid, int? revision = null) {
            WorkflowDetails workflow = await workflowservice.GetWorkflow(workflowid, revision);

            return new WorkflowStructure {
                Name = workflow.Name,
                Nodes = workflow.Nodes.Cast<NodeData>().ToArray(),
                Transitions = workflow.Transitions.Select(t => Translate(t, workflow.Nodes)).ToArray()
            };
        }

        int FindIndex<T>(IEnumerable<T> collection, Func<T, bool> predicate) {
            int index = 0;
            foreach (T item in collection) {
                if (predicate(item))
                    return index;
                ++index;
            }

            throw new NotFoundException(typeof(T), predicate.ToString());
        }
        
        IndexTransition Translate(TransitionData transition, NodeDetails[] nodes) {
            return new IndexTransition {
                Type = transition.Type,
                OriginIndex = FindIndex(nodes, n => n.Id == transition.OriginId),
                TargetIndex = FindIndex(nodes, n => n.Id == transition.TargetId),
                Condition = transition.Condition,
                Log = transition.Log
            };
        }
    }
}