import { Component, OnInit, OnDestroy } from '@angular/core';
import { NavigationItem } from '../dto/navigation/navigationItem';
import { ScheduledTaskService } from '../services/scheduled-task.service';
import { ScheduledTask } from '../dto/tasks/scheduledtask';
import { Subscription, timer } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-scheduled-tasks',
  templateUrl: './scheduled-tasks.component.html',
  styleUrls: ['./scheduled-tasks.component.css']
})
export class ScheduledTasksComponent implements OnInit, OnDestroy {
  navigationPath: NavigationItem[]=[
    {display: "Scheduler"}
  ]

  tasks: ScheduledTask[];
  reloadsub: Subscription;
  page: number;
  pages: number;

  constructor(private scheduledtaskservice: ScheduledTaskService, private router: Router) { }

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
      this.tasks=t.result;

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
  deleteTask(taskId: number): void {
    this.scheduledtaskservice.delete(taskId).subscribe(s=>{
      this.listTasks();
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
