export class Errors {

    /**
     * extracts message text of an error object
     * @param error error of which to get text
     */
    public static getErrorText(error: any): string {
        if(!error)
            return "unknown error";
        
        if(error.error && error.error.text)
            return error.error.text;
        
        return error.toString();
    }
}