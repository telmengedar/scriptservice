import { Time } from '@angular/common';

export interface WorkableTask {
    id: string,
    workableId: number,
    workableRevision: number,
    workableName: string,
    parameters: any,
    log: string[],
    started: Date,
    finished?: Date,
    runtime: Time,
    status: string,
    result: any
}