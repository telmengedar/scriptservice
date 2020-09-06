import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { LoginService } from '../services/login.service';
import { User } from '../dto/user';
import {environment} from 'src/environments/environment';

/**
 * checks whether authentication is required and if, checks required roles
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  requiresLogin: boolean;

  /**
   * creates a new instance
   * @param router access to routing information
   * @param loginservice provides login functionality
   */
  constructor(private router: Router, private loginservice: LoginService) {
    this.requiresLogin=environment.requiresLogin;
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if(!this.requiresLogin)
      return true;
    
    let user: User=this.loginservice.getUser();
    if(user&&user.roles.indexOf("Mamgo-Developer")>-1)
      return true;

    this.loginservice.logout();
    this.router.navigate(['/login'], {queryParams: {returnUrl: state.url}});
    return false;
  }  
}
