import { Injectable } from '@angular/core';
import {Page} from '../dto/page';
import {WorkableTask} from '../dto/workabletask';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import {environment} from 'src/environments/environment'
import { TaskFilter } from '../dto/scripttaskfilter';
import { Rest } from '../helpers/rest';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private tasksurl=`${environment.environment.API_URL}/v1/tasks`;

  constructor(private http: HttpClient) { }

  /**
   * lists existing script tasks
   * @param filter filter to apply to listing
   */
  listTasks(filter?: TaskFilter): Observable<Page<WorkableTask>> {
    return this.http.get<Page<WorkableTask>>(this.tasksurl, {
      params: Rest.createParameters(filter)
    });
  }

  /**
   * cancels execution of a script task
   * @param taskid id of task to cancel
   */
  cancelTask(taskid: string): Observable<Object> {
    const taskurl=`${this.tasksurl}/${taskid}`;
    return this.http.delete(taskurl);
  }

  /**
   * get details of a script task
   * @param taskid id of task to get
   */
  getTask(taskid: string): Observable<WorkableTask> {
    const taskurl=`${this.tasksurl}/${taskid}`;
    return this.http.get<WorkableTask>(taskurl);
  }
}
