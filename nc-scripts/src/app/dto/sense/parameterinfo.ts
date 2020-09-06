import { PropertyInfo } from './propertyinfo';

/**
 * info about a method parameter
 */
export interface ParameterInfo extends PropertyInfo {
    
    /**
     * determines whether parameter is a reference parameter
     */
    isReference?: boolean,

    /**
     * determines whether parameter has a default value
     */
    hasDefault?: boolean,

    /**
     * determines whether parameter is a params array
     */
    isParams?: boolean
}