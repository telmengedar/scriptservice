import {ListFilter} from './listfilter'

export interface TaskFilter extends ListFilter {
    status: string[],
    from?: Date,
    to?: Date,
    workableId?: number,
    workableName?: string
}