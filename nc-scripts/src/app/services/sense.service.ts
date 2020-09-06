import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { PropertyInfo } from '../dto/sense/propertyinfo';
import { TypeInfo } from '../dto/sense/typeinfo';
import { Observable } from 'rxjs';

/**
 * service providing meta information about types and installed host providers
 */
@Injectable({
  providedIn: 'root'
})
export class SenseService {
  private senseurl=`${environment.apiUrl}/v1/sense`;

  constructor(private http: HttpClient) { }

  /**
   * lists all installed host providers
   */
  listHosts(): Observable<PropertyInfo[]> {
    return this.http.get<PropertyInfo[]>(`${this.senseurl}/hosts`);
  }

  /**
   * lists all installed host providers
   * @param hostname name of host of which to return type info
   */
  getHostInfo(hostname: string): Observable<TypeInfo> {
    return this.http.get<TypeInfo>(`${this.senseurl}/hosts/${hostname}`);
  }

  /**
   * lists all installed host providers
   */
  listTypes(): Observable<PropertyInfo[]> {
    return this.http.get<PropertyInfo[]>(`${this.senseurl}/types`);
  }

  /**
   * get type information
   * @param type name of type of which to return meta info
   */
  getTypeInfo(type: string): Observable<TypeInfo> {
    return this.http.get<TypeInfo>(`${this.senseurl}/types/${type}`);
  }
}
