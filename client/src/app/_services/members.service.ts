import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

/**
 * this "Injectable" means this service can be injected into other components  
 * and other services in our app
 */
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getMembers(){
    return this.http.get<Member[]>(this.baseUrl+'users')
  }

  getMember(username: string){
    return this.http.get<Member>(this.baseUrl+'users/'+username);
  }
}
