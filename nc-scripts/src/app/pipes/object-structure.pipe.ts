import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'objectStructure'
})
export class ObjectStructurePipe implements PipeTransform {

  private extractValue(value: any, depth: number): string {
    let result: string="";
    if(Array.isArray(value)) {
      result+="\n";
      result+=this.extractArray(value, depth+2);
    }
    else if(typeof value === 'object') {
      result+="\n";
      result+=this.extractObject(value, depth+2);
    }
    else if(typeof value === 'string') {
      if(value.length>1024)
        result+=": <textblock>\n";
      else result+=`: ${value}\n`;
    }
    else result+=`: ${value}\n`;
    return result;
  }

  private extractArray(array: any, depth: number): string {
    let result: string="";
    let index=0;
    for(let item of array) {
      result+=" ".repeat(depth);
      result+=`[${index++}]`;
      result+=this.extractValue(item, depth);
    }
    return result;
  }

  private extractObject(object: any, depth: number) : string {
    let result: string="";
    for(let [key, value] of Object.entries(object)) {
      result+=" ".repeat(depth);
      result+=key;
      result+=this.extractValue(value, depth);
    }
    return result;
  }

  transform(value: any, args?: any): any {
    if(!value) return "undefined";

    if(!(typeof value === 'object'))
      return value.toString();

    return this.extractObject(value, 0);
  }
}
