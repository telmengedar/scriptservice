import { Injectable } from '@angular/core';
import {environment} from 'src/environments/environment'
import { HttpClient, HttpParams } from '@angular/common/http';
import {Script} from '../dto/scripts/script'
import {Page} from '../dto/page'
import { Observable } from 'rxjs';
import { ListFilter } from '../dto/listfilter';
import { PatchOperation } from '../dto/patchoperation';
import { WorkableTask } from '../dto/workabletask';
import { ExecuteScriptParameters } from '../dto/executescriptparameters';

@Injectable({
  providedIn: 'root'
})
export class ScriptService {
  private scriptsurl=`${environment.environment.API_URL}/v1/scripts`;

  constructor(private http: HttpClient) { }

  /**
   * creates a new script
   * @param data script data to create
   */
  createScript(data: Script): Observable<Script> {
    return this.http.post<Script>(this.scriptsurl, data);
  }

  /**
   * lists script from script server
   * @param filter filter for list items to match
   */
  listScripts(filter: ListFilter): Observable<Page<Script>> {
    let params=new HttpParams();
    if(filter.continue)
      params=params.append("continue", filter.continue.toString())
    if(filter.query)
      params=params.append("query", filter.query)
    return this.http.get<Page<Script>>(this.scriptsurl, {
      params: params
    });
  }

  /**
   * get a script from server
   * @param scriptId id of script to retrieve
   */
  getScript(scriptId: number): Observable<Script> {
    return this.http.get<Script>(`${this.scriptsurl}/${scriptId}`);
  }

    /**
   * get a script from server
   * @param scriptId id of script to retrieve
   */
  getScriptRevision(scriptId: number, revision: number): Observable<Script> {
    return this.http.get<Script>(`${this.scriptsurl}/${scriptId}/${revision}`);
  }

  /**
   * patches properties of a script
   * @param scriptId id of script to patch
   * @param patches patches to apply
   */
  patchScript(scriptId: number, patches: PatchOperation[]): Observable<Script> {
    return this.http.patch<Script>(`${this.scriptsurl}/${scriptId}`, patches);
  }

  /**
   * deletes a script on script server
   * @param scriptId id of script to delete
   */
  deleteScript(scriptId: number): Observable<object> {
    return this.http.delete(`${this.scriptsurl}/${scriptId}`);
  }

      /**
   * executes a script on script service
   * @param script parameters for script to execute
   */
  execute(script: ExecuteScriptParameters): Observable<WorkableTask> {
    return this.http.post<WorkableTask>(`${this.scriptsurl}/tasks` , script);
  }
}
