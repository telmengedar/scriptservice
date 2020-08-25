export interface Page<T> {
    result: T[],
    total: number,
    continue?: number
}