import { SenseEntry } from './senseentry';
import { ParameterInfo } from './parameterinfo';

/**
 * info about a method
 */
export interface MethodInfo extends SenseEntry {

    /**
     * parameters of method
     */
    parameters: ParameterInfo[]

    /**
     * returned type
     */
    returns?: string
}