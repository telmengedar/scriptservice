import { IndexTransition } from "../dto/workflows/indexTransition";
import { Transition } from "../dto/workflows/transition";
import { WorkflowDetails } from "../dto/workflows/workflowdetails";
import { WorkflowNode } from "../dto/workflows/workflownode";
import { WorkflowStructure } from "../dto/workflows/workflowstructure";

/**
 * methods used to interact with workflows
 */
export class Workflows {

    private static buildTransitions(nodes: WorkflowNode[], transitions: Transition[]): IndexTransition[] {
        let result: IndexTransition[]=[];
        transitions.forEach(t=>{
          result.push({
            originIndex: nodes.findIndex(n=>n.id===t.originId),
            targetIndex: nodes.findIndex(n=>n.id===t.targetId),
            condition: t.condition,
            type: t.type,
            log: t.log
          });
        });
        return result;
      }
 
    /**
     * converts a workflow to a structure which can be used for create and update
     * @param workflow workflow to convert to a workflow structure
     */
    public static toStructure(workflow: WorkflowDetails): WorkflowStructure {
        return {
            scope: workflow.scope,
            name: workflow.name,
            nodes: workflow.nodes,
            transitions: Workflows.buildTransitions(workflow.nodes, workflow.transitions)
          };
    }
}