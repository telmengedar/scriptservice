import { ScopedCode } from './scopedcode';
import { TimeSpan } from './timespan';

/**
 * parameters for script execution
 */
export interface ExecuteScriptParameters {
    id?: number,
    name?: string,
    code?: ScopedCode,
    parameters?: any,
    wait?: TimeSpan
}