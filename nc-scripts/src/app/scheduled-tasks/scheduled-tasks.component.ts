import { Component, OnInit, OnDestroy } from '@angular/core';
import { ScheduledTaskService } from '../services/scheduled-task.service';
import { ScheduledTask } from '../dto/tasks/scheduledtask';
import { Subscription, timer } from 'rxjs';
import { Router } from '@angular/router';
import { MatDialog, MatTableDataSource } from '@angular/material';
import { Column } from '../dto/column';
import { ConfirmDeleteComponent } from '../dialogs/confirm-delete/confirm-delete.component';
import { Paging } from '../helpers/paging';
import { Tables } from '../helpers/tables';

@Component({
  selector: 'app-scheduled-tasks',
  templateUrl: './scheduled-tasks.component.html',
  styleUrls: ['./scheduled-tasks.component.css']
})
export class ScheduledTasksComponent implements OnInit, OnDestroy {
  Paging=Paging;
  Tables=Tables;

  tasks: MatTableDataSource<ScheduledTask>=new MatTableDataSource<ScheduledTask>();
  reloadsub: Subscription;
  page: number;
  pages: number;

  columns: Column[]=[
    {
      display: "Name",
      name: "name",
    },
    {
      display: "Type",
      name: "workableType"
    },
    {
      display: "Workable",
      name: "workableName",
    },
    {
      display: "Revision",
      name: "workableRevision",
    },
    {
      display: "Interval",
      name: "interval",
    },
    {
      display: "Next Run",
      name: "target",
      format: "Date"
    },
    {
      display: "",
      name: "_actions",
    }
  ]

  constructor(private scheduledtaskservice: ScheduledTaskService, private router: Router, private dialog: MatDialog) { }

  ngOnDestroy(): void {
    this.reloadsub.unsubscribe();
  }

  ngOnInit() {
    this.listTasks();
    this.reloadsub=timer(5000, 5000).subscribe(t=>this.listTasks());
  }

  /**
   * reloads current page of scheduled tasks
   */
  listTasks() : void {
    this.scheduledtaskservice.list().subscribe(t=>{
      this.tasks.data=t.result;

      this.pages=Math.ceil(t.total/50);
      if(t.continue) {
        this.page=Math.floor(t.continue/50)
      }
      else this.page=this.pages;
    });
  }

  /**
   * deletes a scheduled task
   * @param taskId id of task to delete
   */
  deleteTask(task: ScheduledTask): void {
    const dialogRef=this.dialog.open(ConfirmDeleteComponent, {
      data: task
    });

    dialogRef.afterClosed().subscribe(r=>{
      if(r) {
        this.scheduledtaskservice.delete(task.id).subscribe(s=>{
          this.listTasks();
        });
      }
    });
  }

  /**
   * changes to the page used to create a new scheduler task
   */
  newTask(): void {
    this.router.navigateByUrl(`/scheduler/create`);
  }

  /**
   * changes to the page used to edit a scheduled task
   * @param taskId id of task to edit
   */
  editTask(taskId: number): void {
    this.router.navigateByUrl(`/scheduler/${taskId}`);
  }
}
