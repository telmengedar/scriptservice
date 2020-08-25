/**
 * scriptcode which can get executed on server
 */
export interface Script {
    id?: number,
    revision?: number,
    name: string,
    scope: string,
    code: string
}