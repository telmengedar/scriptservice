/**
 * supported script code languages
 */
export enum ScriptLanguage {

    /**
     * nightlycode script language
     */
    NCScript=0,

    /**
     * javascript
     */
    JavaScript=1
}

export namespace ScriptLanguage {

    export function getNodeTypeName(type: any): string {
        if(typeof type === "number")
            return ScriptLanguage[type];

        if(typeof type === "string")
        {
            let num=parseInt(type)
            if(num>=0)
                return ScriptLanguage[num];
        }

        return type.toString();
    }

    export function getNodeTypeValue(type: any): ScriptLanguage {
        if(typeof type === "number")
            return type as number;

        if(typeof type === "string") {
            const num=parseInt(type);
            if(num>=0)
                return num;
            return ScriptLanguage[type as string];
        }
        return type;
    }
}