import { MethodInfo } from './methodinfo';
import { PropertyInfo } from './propertyinfo';
import { SenseEntry } from './senseentry';

/**
 * info about a type
 */
export interface TypeInfo extends SenseEntry {

    /**
     * type of element if this type is a collection
     */
    elementType?: string,

    /**
     * methods available in type
     */
    methods: MethodInfo[],

    /**
     * properties available in type
     */
    properties: PropertyInfo[],

    /**
     * indexer methods 
     */
    indexer: MethodInfo[]
}