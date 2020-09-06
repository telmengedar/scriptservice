import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthToken } from '../dto/authtoken';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { 
  }

  getToken(username: string, password: string): Observable<AuthToken> {
    let body=new URLSearchParams();
    let options = {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
    };
    
    body.set("grant_type", "password");
    body.set("username", username);
    body.set("password", password);
    body.set("client_id", appprocess.env.CLIENT_ID);
    body.set("client_secret", appprocess.env.CLIENT_SECRET);

    return this.http.post<AuthToken>(appprocess.env.AUTH_URL, body.toString(), options);
  }

  refreshToken(token: string): Observable<AuthToken> {
    let body=new URLSearchParams();
    let options = {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
    };

    body.set("grant_type", "refresh_token");
    body.set("refresh_token", token);
    body.set("client_id", appprocess.env.CLIENT_ID);
    body.set("client_secret", appprocess.env.CLIENT_SECRET);

    return this.http.post<AuthToken>(appprocess.env.AUTH_URL, body.toString(), options);
  }
}
