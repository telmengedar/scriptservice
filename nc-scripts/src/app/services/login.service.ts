import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { User } from '../dto/user';
import { Observable } from 'rxjs';
import {map} from 'rxjs/operators'
import { AuthToken } from '../dto/authtoken';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  user: User;

  constructor(private authservice: AuthService) { }

  login(username: string, password: string): Observable<User> {
    return this.authservice.getToken(username, password).pipe(map(t=>{
        return this.handleUserResponse(t);
      }));
  }

  getUser(): User {
    return this.user;
  }

  refresh(): Observable<User> {
    if(this.user==null)
      return;

    const token=this.user.refresh;
    const expiresin=this.user.refreshexpires;
    this.logout();

    if(Date.now()<expiresin) {
      return this.authservice.refreshToken(token).pipe(map(t=>{
        return this.handleUserResponse(t);
      }));
    }
  }

  logout(): void {
    this.user=null;
  }

  private handleUserResponse(response: AuthToken): User {
    let userobject=JSON.parse(atob(response.access_token.split('.')[1]));
    this.user={
      token: response.access_token,
      name: userobject.name,
      roles: userobject.realm_access.roles,
      refresh: response.refresh_token,
      expires: Date.now()+response.expires_in*800,
      refreshexpires: Date.now()+response.refresh_expires_in*1000
    };
    return this.user; 
  }
}
