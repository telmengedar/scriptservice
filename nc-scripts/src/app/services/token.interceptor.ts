import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginService } from './login.service';
import { User } from '../dto/user';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
    constructor(private loginservice: LoginService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const user: User=this.loginservice.getUser();
        if(user)
        {
            if(Date.now()>user.expires){
                this.loginservice.refresh().toPromise().then(u=>{
                    return next.handle(request.clone({
                        headers: request.headers.set("Authorization", `Bearer ${u.token}`)
                    }));
                });
            }
            else {
                return next.handle(request.clone({
                    headers: request.headers.set("Authorization", `Bearer ${user.token}`)
                }));
            }   
        }

        return next.handle(request);
    }
}