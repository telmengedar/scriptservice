import { Component, OnInit, OnDestroy } from '@angular/core';
import { WorkableTask } from '../dto/workabletask';
import {TaskService} from '../services/task.service'
import { Router } from '@angular/router';
import { Page } from '../dto/page';
import { Column } from '../dto/column';
import { MatTableDataSource } from '@angular/material';
import { Paging } from '../helpers/paging';
import { TaskFilter } from '../dto/scripttaskfilter';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})

export class TasksComponent implements OnInit, OnDestroy {
  Paging=Paging;

  page: number=0;
  total: number=0;

  isdestroyed:boolean=false;

  filter: TaskFilter={
    status: [],
    count: 15
  }
  tasks: MatTableDataSource<WorkableTask>=new MatTableDataSource<WorkableTask>();

  columns: Column[]=[
    {
      display: "Name",
      name: "workableName",
    },
    {
      display: "Started",
      name: "started",
      format: "Date"
    },
    {
      display: "Runtime",
      name: "runtime",
    },
    {
      display: "Status",
      name: "status",
    },
    {
      display: "Result",
      name: "result",
    },
    {
      display: "",
      name: "_actions",
    }
  ]

  constructor(private scripttaskservice: TaskService, private router: Router) { }

  ngOnInit() {
    this.listTasks(true);
  }

  ngOnDestroy() {
    this.isdestroyed=true;
  }

  showDetails(taskId: string): void {
    this.router.navigateByUrl(`/tasks/${taskId}`);
  }

  listTasks(timer: boolean): void {
    this.scripttaskservice.listTasks(this.filter).subscribe(t=>this.pageReceived(t, timer));
  }

  private pageReceived(task: Page<WorkableTask>, timer: boolean): void {
    this.tasks.data=task.result;
    this.total=task.total;
    if(task.continue) {
      this.page=Math.floor(task.continue/this.filter.count)-1;
    }
    else {
      this.page=task.total/this.filter.count;
    }

    if(timer && !this.isdestroyed)
      setTimeout(()=>this.listTasks(true), 1000);
  }

  cancelTask(taskid: string): void {
    this.scripttaskservice.cancelTask(taskid).subscribe(r=>this.listTasks(false));
  }

  hasStatus(status: string): boolean {
    return this.filter.status.indexOf(status)>=0;
  }

  changeStatus(status: string): void {
    let index=this.filter.status.indexOf(status);
    if(index>=0)
      this.filter.status.splice(index, 1);
    else this.filter.status.push(status);
    this.listTasks(false);
  }

  changeFrom(from: Date): void {
    this.filter.from=from;
    this.listTasks(false);
  }

  changeTo(to: Date): void {
    this.filter.to=to;
    this.listTasks(false);
  }

  getDisplayedColumns(): string[] {
    return this.columns.map<string>(c=>c.name);
  }
}
