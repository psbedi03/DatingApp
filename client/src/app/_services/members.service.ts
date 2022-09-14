import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

/**
 * this "Injectable" means this service can be injected into other components  
 * and other services in our app
 */
@Injectable({
  providedIn: 'root'
})
/**
 * Services are Singletons, they are instantiated when component needs a service
 * Thier instance stays alive unitl application is closed
 * 
 * So Services are good example to store application state
 * 
 * There are other state mngmnt solutions, such as Redux, Mobex
 * But we will be over killing if we use them
 * because angular provide us these services to do what we are looking for
 */
export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers(){
    if(this.members.length > 0){
      //return as observable so that our component/client can observe this data
      return of(this.members)
    }
    return this.http.get<Member[]>(this.baseUrl+'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    )
  }

  getMember(username: string){
    const member = this.members.find(x => x.userName === username);
    if(member !== undefined) return of(member);

    return this.http.get<Member>(this.baseUrl+'users/'+username);
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl+"users", member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }
}
