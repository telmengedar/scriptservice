
/**
 * helper methods for script parameters
 */
export class Parameters {

    /**
     * translates properties of a parameter object trying to read properties in json notification
     * @param parameters parameter object to translate
     */
    public static translate(parameters: any) : any {
        var translated: any={}
        for (const [key, value] of Object.entries(parameters)) {
            if(typeof value === "string")
            {
                try {
                    translated[key]=JSON.parse(value);
                }
                catch {
                    translated[key]=value;
                }
            }
            else {
                translated[key]=value;
            }
        }

        return translated;
    }
}