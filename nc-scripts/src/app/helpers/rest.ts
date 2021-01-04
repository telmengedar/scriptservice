import { HttpParams } from '@angular/common/http';

/**
 * provides helper methods for rest interaction
 */
export class Rest {

    /**
     * creates http parameters from a filter class
     * @param filter filter to convert to parameters
     */
    public static createParameters(filter: any): HttpParams {
        if(!filter)
            return null;
        
        let params=new HttpParams();

        for(const [key,value] of Object.entries(filter)) {
            if(Array.isArray(value)) {
                for(const item of value) {
                    params=params.append(key.toLowerCase(), item.toString());
                }
            }
            else {
                params=params.append(key.toLowerCase(), value.toString());
            }
        }

        return params;
    }
}