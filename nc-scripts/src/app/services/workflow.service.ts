import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { WorkflowStructure } from '../dto/workflows/workflowstructure';
import { WorkflowDetails } from '../dto/workflows/workflowdetails';
import { Observable } from 'rxjs';
import { ListFilter } from '../dto/listfilter';
import { Workflow } from '../dto/workflows/workflow';
import { Page } from '../dto/page';
import { PatchOperation } from '../dto/patchoperation';
import { ExecuteWorkflowParameters } from '../dto/workflows/executeworkflowparameters';
import { WorkableTask } from '../dto/workabletask';
import { ContinueWorkflowParameters } from '../dto/workflows/continueworkflowparameters';

/**
 * service used to interact with workflows in backend
 */
@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
  private workflowsurl=`${environment.environment.API_URL}/v1/workflows`;
  
  /**
   * creates a new {@link WorkflowService}
   */
  constructor(private http: HttpClient) { }

  /**
   * creates a new workflow
   * @param workflow structure of workflow to be created
   */
  createWorkflow(workflow: WorkflowStructure): Observable<WorkflowDetails> {
    return this.http.post<WorkflowDetails>(this.workflowsurl, workflow);
  }

  /**
   * updates a workflow with complete structural data
   * @param workflowid id of workflow to update
   * @param workflow structure for workflow to contain
   */
  updateWorkflow(workflowid: number, workflow: WorkflowStructure): Observable<WorkflowDetails> {
    return this.http.put<WorkflowDetails>(`${this.workflowsurl}/${workflowid}`, workflow);
  }

  /**
   * get a workflow from backend by specifying its id
   * @param workflowid id of workflow to get
   */
  getWorkflowById(workflowid: number): Observable<WorkflowDetails> {
    return this.http.get<WorkflowDetails>(`${this.workflowsurl}/${workflowid}`);
  }

  /**
   * get a workflow from backend by specifying its id
   * @param workflowid id of workflow to get
   * @param revision revision of workflow to load
   */
  getWorkflowRevision(workflowid: number, revision: number): Observable<WorkflowDetails> {
    return this.http.get<WorkflowDetails>(`${this.workflowsurl}/${workflowid}/${revision}`);
  }
  
  /**
   * lists workflows in backend
   * @param filter filter to apply
   */
  listWorkflows(filter?: ListFilter): Observable<Page<Workflow>> {
    return this.http.get<Page<Workflow>>(this.workflowsurl);
  }

  /**
   * patches properties of a workflow
   * @param workflowid id of workflow to patch
   * @param patches patches to apply
   */
  patchWOrkflow(workflowid: number, patches: PatchOperation[]): Observable<WorkflowDetails> {
    return this.http.patch<WorkflowDetails>(`${this.workflowsurl}/${workflowid}`, patches);
  }

  /**
   * deletes a workflow in backend
   * @param workflowid id of workflow to delete
   */
  deleteWorkflow(workflowid: number): Observable<any> {
    return this.http.delete(`${this.workflowsurl}/${workflowid}`)
  }

  /**
   * executes a workflow on server
   * @param workflow parameters defining workflow to execute
   */
  execute(workflow: ExecuteWorkflowParameters): Observable<WorkableTask> {
    return this.http.post<WorkableTask>(`${this.workflowsurl}/tasks`, workflow);
  }

  /**
   * continues execution of a suspended workflow on server
   * @param taskid id of task to continue
   * @param workflow parameters defining workflow to execute
   */
  continue(taskid: string, parameters: ContinueWorkflowParameters): Observable<WorkableTask> {
    return this.http.put<WorkableTask>(`${this.workflowsurl}/tasks/${taskid}`, parameters);
  }
}
