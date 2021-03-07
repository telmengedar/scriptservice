import { Column } from "../dto/column";

/**
 * data operations for table structures
 */
export class Tables {

    /**
     * get properties of columns to display
     * @param columns column descriptors to extract properties from
     */
    public static getColumnProperties(columns: Column[]): string[] {
        return columns.map<string>(c=>c.name);
    }
}