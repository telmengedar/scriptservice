import { Component, OnInit } from '@angular/core';
import { NavigationItem } from '../dto/navigation/navigationItem';
import { ScheduledTask } from '../dto/tasks/scheduledtask';
import { ScheduledDays } from '../dto/tasks/scheduleddays';
import { WorkableType } from '../dto/tasks/workabletype';
import { ActivatedRoute } from '@angular/router';
import { ScheduledTaskService } from '../services/scheduled-task.service';
import { Patch } from '../helpers/patch';

@Component({
  selector: 'app-scheduled-task-editor',
  templateUrl: './scheduled-task-editor.component.html',
  styleUrls: ['./scheduled-task-editor.component.css']
})
export class ScheduledTaskEditorComponent implements OnInit {
  ScheduledDays=ScheduledDays;

  original: ScheduledTask;
  task: ScheduledTask;
  changed: boolean;
  
  workableTypes= [
    {
      type: WorkableType.Script,
      name: "Script",
      description: "Executes an NC-Script"
    },
    {
      type: WorkableType.Workflow,
      name: "Workflow",
      description: "Executes a Workflow"
    }
  ];

  constructor(private scheduledTaskService: ScheduledTaskService, route: ActivatedRoute) { 
    this.task={
      name: "",
      days: ScheduledDays.All,
      workableType: WorkableType.Script,
      workableName: ""
    }

    if(route.snapshot.params.taskId!=="create") {
      this.task.id=route.snapshot.params.taskId;
      this.scheduledTaskService.getById(this.task.id).subscribe(t=>this.handleTaskResult(t));
    }
  }

  ngOnInit() {
  }

  private handleTaskResult(task: ScheduledTask) {
    this.task=task;
    this.task.workableType=WorkableType.getValue(this.task.workableType);
    this.task.days=ScheduledDays.getValue(this.task.days);
    this.original=Object.assign({}, this.task);
    this.changed=false;
  }

  /**
   * determines whether the flag array in task contains the flag for a specific day
   * @param day day to check for
   */
  hasDay(day: ScheduledDays): boolean {
    let taskvalue=ScheduledDays.getValue(this.task.days);
    let dayvalue=ScheduledDays.getValue(day);
    return (dayvalue&taskvalue)!==0;
  }

  /**
   * toggles the indicator for a day in the task day flag array
   * @param day day to toggle
   */
  toggleDay(day: ScheduledDays): void {
    if(this.hasDay(day)) {
      this.task.days=ScheduledDays.getValue(this.task.days)&(~ScheduledDays.getValue(day));
    } else {
      this.task.days=ScheduledDays.getValue(this.task.days)|ScheduledDays.getValue(day);
    }
    this.changed=true;
  }

  /**
   * saves the current task
   */
  save(): void {
    if(!this.task.id) {
      this.scheduledTaskService.create(this.task).subscribe(t=>this.handleTaskResult(t));
    } else {
      this.scheduledTaskService.patch(this.task.id, Patch.generatePatches(this.original, this.task)).subscribe(t=>this.handleTaskResult(t));
    }
  }
}
