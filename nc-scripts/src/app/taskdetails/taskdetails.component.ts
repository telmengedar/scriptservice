import { Component, OnInit, OnDestroy } from '@angular/core';
import {Location} from '@angular/common';
import { WorkableTask } from '../dto/workabletask';
import { TaskService } from '../services/task.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-taskdetails',
  templateUrl: './taskdetails.component.html',
  styleUrls: ['./taskdetails.component.css'],
  
})
export class TaskDetailsComponent implements OnInit, OnDestroy {
  taskid: string;
  task: WorkableTask;
  styledlog: string[];
  isdestroyed:boolean=false;

  constructor(private scripttaskservice: TaskService, route: ActivatedRoute, private location: Location) { 
    this.taskid=route.snapshot.params.taskId;
  }

  ngOnInit() {
    this.loadTask();
  }

  ngOnDestroy() {
    this.isdestroyed=true;
  }

  loadTask(): void {
    this.scripttaskservice.getTask(this.taskid).subscribe(t=>this.taskLoaded(t));
  }

  private taskLoaded(task: WorkableTask): void {
    if(this.isdestroyed)
      return;

    this.task=task;
    this.styledlog=[];
    if(this.task.log) {
      this.task.log.forEach(l=>{
        l.split("\n").forEach(s=>{
          this.styledlog.push(s);
        })
      })
    }

    if(task.status==="Running")
    {
      console.log("setting timeout");
      setTimeout(()=>this.loadTask(), 500);
    }
  }

  back(): void {
    this.location.back();
  }
}
