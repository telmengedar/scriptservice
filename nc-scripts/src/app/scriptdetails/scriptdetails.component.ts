import { Component, OnInit, OnDestroy } from '@angular/core';
import { Script } from '../dto/script';
import {Location} from '@angular/common';
import { ScriptService } from '../services/script.service';
import { PatchOperation } from '../dto/patchoperation';
import { Patch } from '../helpers/patch';
import { ActivatedRoute } from '@angular/router';
import { WorkableTask } from '../dto/workabletask';
import { TaskService } from '../services/task.service';

/**
 * editor component for a single script.
 * Allows to edit an existing script or, when called without a script id, creates a new script on save.
 */
@Component({
  selector: 'app-scriptdetails',
  templateUrl: './scriptdetails.component.html',
  styleUrls: ['./scriptdetails.component.css']
})
export class ScriptDetailsComponent implements OnInit, OnDestroy {
  script: Script={
    scope: "",
    name: "",
    code: ""
  };
  oldscript: Script;

  parameters: any={};
  task: WorkableTask;

  editorOptions = {
    theme: 'vs-dark', 
    language: 'csharp',
    fontSize: "16px",
    scrollBeyondLastLine: false,
    minimap: {
      enabled: false
    },
    scrollbar: {
      vertical: 'hidden',
      horizontal: 'hidden'
    },
    highlightActiveIndentGuide: true,
    renderLineHighlight: "gutter"
  };
  changed:boolean=false;
  taskrunning: boolean=false;
  isdestroyed:boolean=false;

  constructor(private scriptservice: ScriptService, private taskservice: TaskService, private location: Location, route: ActivatedRoute) { 
    if(route.snapshot.params.scriptId!=="create")
      this.script.id=route.snapshot.params.scriptId;
  }

  ngOnInit() {
    if(this.script.id) {
      this.scriptservice.getScript(this.script.id).subscribe(s=>this.scriptLoaded(s));
    }
    else this.oldscript=Object.assign({}, this.script);
  }

  ngOnDestroy() {
    this.isdestroyed=true;
  }

  /**
   * navigates back to the script list page
   */
  back(): void {
    this.location.back();
  }

  /**
   * saves the current script
   */
  save(): void {
    if(this.script.id)
    {
      console.log(this.oldscript);
      console.log(this.script);
      let patches:PatchOperation[]=Patch.generatePatches(this.oldscript, this.script);
      this.scriptservice.patchScript(this.script.id, patches).subscribe(s=>this.scriptLoaded(s));
    }
    else {
      this.scriptservice.createScript(this.script).subscribe(s=>this.scriptLoaded(s));
    }
  }

  /**
   * adds a new parameter for script test execution
   */
  addParameter(name: string): void {
    this.parameters[name]="";
  }

  /**
   * changes value of an existing parameter
   * @param name name of parameter to set
   * @param value value of parameter to set
   */
  setParameter(name: string, value: string): void {
    this.parameters[name]=value;
  }

  /**
   * executes test for the current script with the specified parameters
   */
  executeTest(): void {
    this.taskrunning=true;
    this.scriptservice.execute({
      code: {
        scope: this.script.scope,
        name: this.script.name,
        code: this.script.code
      },
      parameters: this.parameters
    }).subscribe(t=>this.taskLoaded(t));
  }

  private getTaskInfo() {
    if(!this.task)
    {
      this.taskrunning=false;
      return;
    }

    this.taskservice.getTask(this.task.id).subscribe(t=>this.taskLoaded(t));
  }

  private taskLoaded(task: WorkableTask): void {
    if(this.isdestroyed)
      return;
    this.task=task;
    if(task.status==="Running")
    {
      setTimeout(()=>this.getTaskInfo(), 500);
    }
    else this.taskrunning=false;
  }

  private scriptLoaded(script: Script): void {
    this.script=script;
    this.oldscript=Object.assign({}, this.script);
    this.changed=false;
  }
}
