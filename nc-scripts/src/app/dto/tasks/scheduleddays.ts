
/**
 * days when task execution is scheduled
 */
export enum ScheduledDays {

    /**
     * no days selected. Usually leads to all days scheduled except when used as filter
     */
    None=0,

    /**
     * monday
     */
    Monday=1,

    /**
     * tuesday
     */
    Tuesday=2,

    /**
     * wednesday
     */
    Wednesday=4,

    /**
     * thursday
     */
    Thursday=8,

    /**
     * friday
     */
    Friday=16,

    /**
     * saturday
     */
    Saturday=32,

    /**
     * sunday
     */
    Sunday=64,

    /**
     * all days set
     */
    All=127
}

export namespace ScheduledDays {

    export function getName(type: any): string {
        if(type as string)
        {
            let num=parseInt(type)
            if(num>=0)
            return ScheduledDays[num];
        }
        return type;
    }

    export function getValue(type: any): ScheduledDays {
        if(type as string) {
            let num=parseInt(type);
            if(num>=0)
                return num;

            const literal=type as string;
            if(literal.indexOf(", ")>-1)
            {
                const literalarray=literal.split(", ");
                let value=ScheduledDays.None;
                literalarray.forEach(l=>value|=ScheduledDays[l]);
                return value;
            }
            return ScheduledDays[literal];
        }
        return type;
    }
}