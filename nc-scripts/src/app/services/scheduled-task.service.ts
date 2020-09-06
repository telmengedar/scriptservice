import { Injectable } from '@angular/core';
import { RestService } from './restservice.service';
import { ScheduledTask } from '../dto/tasks/scheduledtask';
import { ScheduledTaskData } from '../dto/tasks/scheduledtaskdata';
import { ListFilter } from '../dto/listfilter';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

/**
 * service providing interaction with {@link ScheduledTask} entities
 */
@Injectable({
  providedIn: 'root'
})
export class ScheduledTaskService extends RestService<ScheduledTask, ScheduledTaskData, ListFilter> {

  /**
   * creates a new {@link ScheduledTaskService}
   * @param http http client used to send http requests
   */
  constructor(http: HttpClient) {
    super(`${environment.apiUrl}/v1/scheduler`, http) 
  }
}
