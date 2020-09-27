import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import { NodeData } from 'src/app/dto/workflows/nodeData';
import { PropertyInfo } from 'src/app/dto/sense/propertyinfo';
import { SenseService } from 'src/app/services/sense.service';
import { ImportDeclaration } from 'src/app/dto/workflows/importdeclaration';
import { NodeEditorParameters } from 'src/app/dto/workflows/nodeeditorparameters';
import { MethodInfo } from 'src/app/dto/sense/methodinfo';
import { NodeType } from 'src/app/dto/workflows/nodetype';
import { ParameterInfo } from 'src/app/dto/sense/parameterinfo';
import { ScriptLanguageOptions } from 'src/app/dto/scripts/scriptlanguageoptions';
import { ScriptLanguage } from 'src/app/dto/scripts/scriptlanguage';

@Component({
  selector: 'app-node-editor',
  templateUrl: './node-editor.component.html',
  styleUrls: ['./node-editor.component.css']
})
export class NodeEditorComponent implements OnInit {
  NodeType=NodeType;

  nodetypes=[{
    name: "Node",
    type: NodeType.Node,
    description: "Basic Node with no functionality"
  },{
    name: "Expression",
    type: NodeType.Expression,
    description: "Allows to specify code in NCScript syntax to execute"
  },{
    name: "Binary Operation",
    type: NodeType.BinaryOperation,
    description: "Executes a binary operation on two values"
  },{
    name: "Generate Value",
    type: NodeType.Value,
    description: "Generates a value"
  },{
    name: "Call Method",
    type: NodeType.Call,
    description: "Calls a method on a variable or Script Host"
  },{
    name: "Execute Script",
    type: NodeType.Script,
    description: "Executes a script"
  },{
    name: "Execute Workflow",
    type: NodeType.Workflow,
    description: "Executes another workflow"
  },{
    name: "Suspend Execution",
    type: NodeType.Suspend,
    description: "Suspends execution of this workflow and waits for a continuation trigger"
  },{
    name: "Iterate Collection",
    type: NodeType.Iterator,
    description: "Iterates over a collection and executes loop transitions"
  },{
    name: "Log Message",
    type: NodeType.Log,
    description: "Writes a message to the task log"
  }];

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

  hosts: PropertyInfo[]=[];
  imported: ImportDeclaration[]=[];
  methods: MethodInfo[]=[];

  inputkey: string;
  inputvalue: string;

  node: NodeData;

  parametertemplates: ParameterInfo[];
  languages=ScriptLanguageOptions;

  /**
   * creates a new instance
   * @param dialog reference to dialog
   * @param node node to edit
   */
  constructor(private dialog: MatDialogRef<NodeEditorComponent>, private senseservice: SenseService, @Inject(MAT_DIALOG_DATA)private parameters: NodeEditorParameters) { 
    this.node=parameters.node;
    this.imported=parameters.imports;
    if(!this.node.parameters)
      this.node.parameters={}
    
    this.node.type=NodeType.getNodeTypeValue(this.node.type);
  }

  ngOnInit() {
    this.senseservice.listHosts().subscribe(h=>{
      this.hosts=h;
    });

    this.loadHostMethods();
  }

  /**
   * adds a new import declaration
   * @param target element invoking the event
   */
  addImport(key: string, value: string): void {
    if(!this.node.parameters.imports)
      this.node.parameters.imports=[];
    
    if(!key||!value|| key===""||value==="")
      return;

    let index: number=this.node.parameters.imports.findIndex(i=>i.variable===key || i.name===value);
    if(index>=0)
      return;

    this.node.parameters.imports.push({
      type: "Host",
      variable: key,
      name: value
    });

    this.inputkey="";
    this.inputvalue="";
  }

  /**
   * called when node type was changed
   */
  nodeTypeChanged(): void {
    switch(NodeType.getNodeTypeValue(this.node.type)) {
      case NodeType.Call:
        this.loadHostMethods();
        break;
      case NodeType.Expression:
        if(!this.node.parameters)
          this.node.parameters={};
        if(!this.node.parameters.language)
          this.node.parameters.language=ScriptLanguage.JavaScript;
        break;
    }
  }

  /**
   * loads methods of currently selected host
   */
  loadHostMethods(): void {
    if(NodeType.getNodeTypeValue(this.node.type)!==NodeType.Call) {
      this.methods=[];
      return;
    }

    if(!this.node.parameters.host || this.node.parameters.host==="") {
      console.log("host empty");
      this.methods=[];
      return;
    }

    let hostimport=this.imported.find(i=>i.variable===this.node.parameters.host);
    if(!hostimport)
    {
      console.log("host not found");
      this.methods=[];
      return;
    }

    this.senseservice.getHostInfo(hostimport.name).toPromise().then(h=>{
      this.methods=h.methods;
      this.selectMethod();
    }).catch(e=>{
      console.log(e);
      this.methods=[];
    });
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
   * get name of parameter at specified index or index itself if parameter does not exist
   */
  getArgumentName(index: number): string {
    if(!this.parametertemplates||index<0||index>=this.parametertemplates.length)
      return '?';
    
    return this.parametertemplates[index].name;
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
   * adds a parameter to the workable parameter list
   * @param target element invoking the event
   */
  addWorkableParameter(key: string, value: string): void {
    if(!this.node.parameters.arguments)
      this.node.parameters.arguments={};
    this.node.parameters.arguments[key]=value;
    this.inputkey="";
    this.inputvalue="";
  }

  /**
   * deletes an argument from the argument list
   * @param index index of argument to remove
   */
  deleteArgument(index: number): void {
    this.node.parameters.arguments.splice(index, 1);
  }

  /**
   * deletes a parameter from the workable parameter list
   * @param index index of argument to remove
   */
  deleteWorkableParameter(key: string): void {
    delete this.node.parameters.arguments[key];
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

  /**
   * tries to adjust parameters for currently selected method
   */
  selectMethod(): void {
    if(!this.node.parameters.method)
      return;
    
    let method=this.methods.find(m=>m.name.toLowerCase()===this.node.parameters.method.toLowerCase());
    if(!method)
    {
      this.parametertemplates=[];
      return;
    }

    this.parametertemplates=method.parameters;
    if(!this.node.parameters.arguments)
      this.node.parameters.arguments=[];
    
    // this creates entries for arguments which are determined to be passed to the method
    // but not present.
    if(this.node.parameters.arguments.length<this.parametertemplates.length) {
      for (let index = this.node.parameters.arguments.length; index < this.parametertemplates.length; index++) {
        this.node.parameters.arguments.push("");
      }
    }
  }
}
