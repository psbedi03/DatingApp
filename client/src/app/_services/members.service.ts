import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PagninatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

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

  getMembers(userParams: UserParams){
    // if(this.members.length > 0){
    //   //return as observable so that our component/client can observe this data
    //   return of(this.members)
    // }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);

    return this.getPaginatedResult<Member[]>(this.baseUrl+'users', params)
  }

  private getPaginatedResult<T>(url, params) {
    
    const paginatedResult: PagninatedResult<T> = new PagninatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
    
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

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl+'users/set-main-photo/'+photoId, {})
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);
  }
}
