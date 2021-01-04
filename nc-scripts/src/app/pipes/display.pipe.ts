import { formatDate } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'display'
})
export class DisplayPipe implements PipeTransform {

  transform(value: any, ...args: string[]): string {
    if(!value) return "-";

    if(args.length==0 || !args[0])
      return value.toString();
    
    switch(args[0]){
      case 'Date':
        return formatDate(value, 'yyyy-MM-dd hh:mm', "en-US");
      default:
        return value.toString();
    }
  }
}
