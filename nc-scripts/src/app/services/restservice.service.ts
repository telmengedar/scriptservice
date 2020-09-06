import { PatchOperation } from '../dto/patchoperation';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Page } from '../dto/page';

/**
 * service providing basic rest functionality
 */
export class RestService<TEntity, TData, TFilter> {

  /**
   * creates a new {@link RestService}
   * @param baseurl url which points to root url of entities
   * @param http http client to use for http calls
   */
  constructor(private baseurl: string, private http: HttpClient) { 
  }

  /**
   * creates a new entity
   * @param data entity data
   */
  create(data: TData) : Observable<TEntity> {
    return this.http.post<TEntity>(this.baseurl, data);
  }

  /**
   * lists a page of entities which match an optional filter
   * @param filter filter to apply (optional)
   */
  list(filter?: TFilter) : Observable<Page<TEntity>> {
    let params=new HttpParams();
    if(filter) {
      for(let [key,value] of Object.entries(filter)) {
        params=params.append(key, value.toString().toLowerCase());
      }
    }

    return this.http.get<Page<TEntity>>(this.baseurl, {
      params: params
    });
  }

  /**
   * get an entity by id
   * @param id id of entity to get
   */
  getById(id: number) : Observable<TEntity> {
    return this.http.get<TEntity>(`${this.baseurl}/${id}`);
  }

  /**
   * patches an entity
   * @param id id of entity to patch
   * @param patches patches to apply
   */
  patch(id: number, patches: PatchOperation[]):Observable<TEntity> {
    return this.http.patch<TEntity>(`${this.baseurl}/${id}`, patches);
  }

  /**
   * deletes an entity
   * @param id id of entity to delete
   */
  delete(id: number):Observable<object> {
    return this.http.delete(`${this.baseurl}/${id}`)
  }
}