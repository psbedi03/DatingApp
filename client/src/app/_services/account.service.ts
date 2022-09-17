import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

/**
 * this "Injectable" means this service can be injected into other components  
 * and other services in our app
 * this service is a Singlton
 * it means when we inject it, it will be intialized and it will remain intiliazed
 * until our app is disposed off (eg. user closes the browser or moves away from our app)
 * 
 * */
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1); //1 is the size of the buffer
  currentUser$ = this.currentUserSource.asObservable(); //$ is for convention for the observable

  constructor(private http: HttpClient) { }

  login(model: any){
    //we are going to get UserDto from the below return statement
    /** 
     * 
    return this.http.post(this.baseUrl+'account/login', model);
    **/
    
    //we will transform data before returning, so we use pipe
    return this.http.post(this.baseUrl+'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if(user){
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model: any){
    return this.http.post(this.baseUrl+'account/register', model).pipe(
      map((user: User) => {
        if(user){
          this.setCurrentUser(user);
        }
        // return user;
      })
    )
  }

  setCurrentUser(user: User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
