import { ListFilter } from "../listfilter";

/**
 * filter for workflow listings
 */
export interface WorkflowFilter extends ListFilter {

    /**
     * names to search for
     */
    name?: string[]
}