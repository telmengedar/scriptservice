import { ScriptLanguage } from "../scripts/scriptlanguage";

/**
 * basic data of a workflow
 */
export interface WorkflowData {
    
    /**
     * name of workflow
     */
    name: string,

    /**
     * default script language used for expressions
     */
    language?: ScriptLanguage
}