export interface User {
    token: string,
    name: string,
    roles: string[],
    refresh: string,
    expires: number,
    refreshexpires: number
}