
/**
 * type of logic to be executed
 */
export enum WorkableType {

    /**
     * script in nc-script syntax
     */
    Script,

    /**
     * workflow structure
     */
    Workflow
}

export namespace WorkableType {

    export function getName(type: any): string {
        if(type as string)
        {
            let num=parseInt(type)
            if(num>=0)
            return WorkableType[num];
        }
        return type;
    }

    export function getValue(type: any): WorkableType {
        if(type as string) {
            let num=parseInt(type);
            if(num>=0)
                return num;
            return WorkableType[type as string];
        }
        return type;
    }
}