import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import { NodeData } from 'src/app/dto/workflows/nodeData';

@Component({
  selector: 'app-node-editor',
  templateUrl: './node-editor.component.html',
  styleUrls: ['./node-editor.component.css']
})
export class NodeEditorComponent implements OnInit {

  editorOptions = {
    theme: 'vs-dark', 
    language: 'csharp',
    fontSize: "16px",
    scrollBeyondLastLine: false,
    minimap: {
      enabled: false
    },
    highlightActiveIndentGuide: true,
    renderLineHighlight: "gutter"
  };

  constructor(private dialog: MatDialogRef<NodeEditorComponent>, @Inject(MAT_DIALOG_DATA)private node: NodeData) { 
    if(!node.parameters)
      node.parameters={}
  }

  ngOnInit() {
  }

  /**
   * adds a new import declaration
   * @param target element invoking the event
   */
  addImport(target: any): void {
    if(!this.node.parameters.imports)
      this.node.parameters.imports=[];
    
    let index: number=this.node.parameters.imports.findIndex(i=>i.variable===target.value);
    if(index<0) {
      this.node.parameters.imports.push({
        type: "Host",
        variable: target.value,
        name: ""
      });
    }

    target.value="";
  }

  /**
   * deletes an import declaration
   * @param name name of import variable to remove
   */
  deleteImport(name: string): void {
    let index: number=this.node.parameters.imports.findIndex(i=>i.variable===name);
    if(index>=0)
      this.node.parameters.imports.splice(index, 1);
  }

  /**
   * adds an argument to the argument list
   * @param target element invoking the event
   */
  addArgument(target: any): void {
    if(!this.node.parameters.arguments)
      this.node.parameters.arguments=[];
    this.node.parameters.arguments.push(target.value);
  }

  /**
   * deletes an argument from the argument list
   * @param index index of argument to remove
   */
  deleteArgument(index: number): void {
    this.node.parameters.arguments.splice(index, 1);
  }

  /**
   * adds a new parameter for workflow test execution
   */
  addParameter(target: any): void {
    this.node.parameters[target.value]="";
    target.value="";
  }

  /**
   * changes value of an existing parameter
   * @param name name of parameter to set
   * @param value value of parameter to set
   */
  setParameter(name: string, value: string): void {
    this.node.parameters[name]=value;
  }

  /**
   * removes a test parameter
   * @param name name of parameter to delete
   */
  deleteParameter(name: string): void {
    delete this.node.parameters[name];
  }

}
