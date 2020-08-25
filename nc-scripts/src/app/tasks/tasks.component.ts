import { Component, OnInit, OnDestroy } from '@angular/core';
import { WorkableTask } from '../dto/workabletask';
import {TaskService} from '../services/task.service'
import { Router } from '@angular/router';
import { Page } from '../dto/page';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})

export class TasksComponent implements OnInit, OnDestroy {
  tasks: WorkableTask[];
  page: number=0;
  pages: number=0;

  filterStatus:Set<string>=new Set<string>();
  filterFrom:Date;
  filterTo:Date;

  isdestroyed:boolean=false;

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
    this.scripttaskservice.listTasks({
      status: Array.from(this.filterStatus),
      from: this.filterFrom,
      to: this.filterTo
    }).subscribe(t=>this.pageReceived(t, timer));
  }

  private pageReceived(task: Page<WorkableTask>, timer: boolean): void {
    this.tasks=task.result;
    this.pages=Math.ceil(task.total/50);
    if(task.continue) {
      this.page=Math.floor(task.continue/50)
    }
    else this.page=this.pages;

    if(timer && !this.isdestroyed)
      setTimeout(()=>this.listTasks(true), 1000);
  }

  cancelTask(taskid: string): void {
    this.scripttaskservice.cancelTask(taskid).subscribe(r=>this.listTasks(false));
  }

  hasStatus(status: string): boolean {
    return this.filterStatus.has(status);
  }

  changeStatus(status: string): void {
    if(this.filterStatus.has(status))
      this.filterStatus.delete(status);
    else this.filterStatus.add(status);
    this.listTasks(false);
  }

  changeFrom(from: Date): void {
    this.filterFrom=from;
    this.listTasks(false);
  }

  changeTo(to: Date): void {
    this.filterTo=to;
    this.listTasks(false);
  }
}
