import { PatchOperation } from '../dto/patchoperation';

/**
 * provides methods to use when interacting with {@link PatchOperation}s
 */
export class Patch {

    /**
     * creates a new replace patch
     * @param property name of property to patch
     * @param value new property value to apply
     */
    public static replace(property: string, value: any):PatchOperation {
        return {
            op: "replace",
            path: `/${property.toLowerCase()}`,
            value: value
        };
    }

    /**
     * creates patch operations which reflect the difference between two objects
     * current this only creates 'replace' operations for every property which is different
     * from the original
     * @param original object which contains original property values
     * @param changed object of same type as original which contains changed property values
     */
    public static generatePatches(original: any, changed: any): PatchOperation[] {
        let operations: PatchOperation[]=[];

        for(const [key,value] of Object.entries(original)) {
            if(value!==changed[key])
                operations.push(this.replace(key, changed[key]));
        }

        return operations;
    }
}