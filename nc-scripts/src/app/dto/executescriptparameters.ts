import { ScopedCode } from './scopedcode';
import { TimeSpan } from './timespan';

/**
 * parameters for script execution
 */
export interface ExecuteScriptParameters {

    /**
     * id of script to execute
     */
    id?: number,

    /**
     * name of script to execute
     */
    name?: string,

    /**
     * script code to execute if code is to be executed directly
     */
    code?: ScopedCode,

    /**
     * parameters to use for execution
     */
    parameters?: any,

    /**
     * timespan to wait for immediate results
     * to be used for scripts/workflows which are expected to execute fast
     */
    wait?: TimeSpan
}