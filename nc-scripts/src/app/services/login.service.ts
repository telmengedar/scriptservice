import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { User } from '../dto/user';
import { Observable, Subscription, timer } from 'rxjs';
import {map} from 'rxjs/operators'
import { AuthToken } from '../dto/authtoken';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  user: User;
  refreshsub: Subscription;

  constructor(private authservice: AuthService) { }

  login(username: string, password: string): Observable<User> {
    return this.authservice.getToken(username, password).pipe(map(t=>{
        this.handleUserResponse(t);
        if(this.user)
        {
          this.refreshsub=timer(this.user.expires_in*800).subscribe(t=>this.refresh().subscribe());
        }
        return this.user;
      }));
  }

  getUser(): User {
    return this.user;
  }

  refresh(): Observable<User> {
    if(this.user==null)
    {
      this.logout();
      return;
    }

    const token=this.user.refresh;
    const expiresin=this.user.refreshexpires;

    if(Date.now()<expiresin) {
      return this.authservice.refreshToken(token).pipe(map(t=>{
        this.handleUserResponse(t);
        if(this.user)
          this.refreshsub=timer(this.user.expires_in*800).subscribe(t=>this.refresh().subscribe());
        return this.user;
      }));
    } else {
      this.logout();
    }
  }

  logout(): void {
    this.user=null;
    if(this.refreshsub)
    {
      this.refreshsub.unsubscribe();
      this.refreshsub=null;
    }
  }

  private handleUserResponse(response: AuthToken): User {
    let userobject=JSON.parse(atob(response.access_token.split('.')[1]));
    this.user={
      token: response.access_token,
      name: userobject.name,
      roles: userobject.realm_access.roles,
      refresh: response.refresh_token,
      expires: Date.now()+response.expires_in*800,
      refreshexpires: Date.now()+response.refresh_expires_in*1000,
      expires_in:response.expires_in
    };
    return this.user; 
  }
}
