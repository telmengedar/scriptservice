import { Component, Inject } from '@angular/core';
import { MatSnackBar, MAT_DIALOG_DATA } from '@angular/material';
import { Parameter } from 'src/app/dto/scripts/parameter';
import { Script } from 'src/app/dto/scripts/script';
import { WorkableType } from 'src/app/dto/tasks/workabletype';
import { WorkableTask } from 'src/app/dto/workabletask';
import { WorkflowStructure } from 'src/app/dto/workflows/workflowstructure';
import { Parameters } from 'src/app/helpers/parameters';
import { ScriptService } from 'src/app/services/script.service';
import { TaskService } from 'src/app/services/task.service';
import { WorkflowService } from 'src/app/services/workflow.service';

@Component({
  selector: 'app-test-workable',
  templateUrl: './test-workable.component.html',
  styleUrls: ['./test-workable.component.scss']
})
export class TestWorkableComponent {
  WorkableType=WorkableType;
  parameters: Parameter[]=[];
  task: WorkableTask={
    id: undefined,
    workableRevision: undefined,
    workableId: undefined,
    workableName: undefined,
    parameters: undefined,
    log: [],
    started: undefined,
    runtime: undefined,
    status: "None",
    result: undefined
  };

  constructor(@Inject(MAT_DIALOG_DATA) public data: {type: WorkableType, workable: any}, 
    private scriptService: ScriptService, 
    private workflowService: WorkflowService, 
    private taskService: TaskService,
    private snackbar: MatSnackBar) { }

  startTest(): void {
    this.task.status="Running";
    this.task.log=["Starting Test ..."];

    switch(this.data.type) {
      case WorkableType.Script:
        this.scriptService.execute({
          parameters: Parameters.translate(this.convertParameters()),
          code: {
            name: (<Script>this.data.workable).name+" (Test)",
            code: (<Script>this.data.workable).code,
            language: (<Script>this.data.workable).language,
          }
        }).toPromise().then(task=>{
          this.task=task;
          if(task.status==="Running")
            setTimeout(()=>this.getTaskInfo(), 1000);
        }).catch(e=>{
          this.task.status="Error"
          this.snackbar.open(e.error.text, "Close");
        });
        break;
      case WorkableType.Workflow:
        this.workflowService.execute({
          parameters: Parameters.translate(this.convertParameters()),
          workflow: (<WorkflowStructure>this.data.workable)
        }).toPromise().then(task=>{
          this.task=task;
          if(task.status==="Running")
            setTimeout(()=>this.getTaskInfo(), 1000);
        }).catch(e=>{
          this.task.status="Error"
          this.snackbar.open(e.error.text, "Close");
        });
        break;
    }
  }

  private getTaskInfo() {
    if(!this.task||this.task.status!=="Running")
      return;

    this.taskService.getTask(this.task.id).toPromise()
    .then(task=>{
      console.log(task);
      this.task=task;
      if(task.status==="Running")
        setTimeout(()=>this.getTaskInfo(), 1000);
    }).catch(e=>{
      this.task.status="Error"
      this.snackbar.open(e.error.text, "Close");
    });
  }  

  private convertParameters(): object {
    if(!this.parameters || this.parameters.length===0)
      return undefined;

    let parameterobject: object={}
    for(let parameter of this.parameters) {
      parameterobject[parameter.name]=parameter.value;
    }
    return parameterobject;
  }

  /**
   * adds a parameter
   */
  addParameter(): void {
    this.parameters.push({
      name: "",
      value: ""
    });
  }

  /**
   * removes a parameter
   * @param index index of parameter to remove
   */
  removeParameter(index: number): void {
    this.parameters.splice(index, 1);
  }
}
