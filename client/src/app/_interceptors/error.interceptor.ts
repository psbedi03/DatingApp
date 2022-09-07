import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      //catchError is rxjs operator
      catchError(error => {
        if(error){//check if error exist
          switch (error.status) {
            case 400:
              if(error.error.errors){
                const modalStatErrors = [];
                for(const key in error.error.errors){
                  modalStatErrors.push(error.error.errors[key]);
                }
                //flat will return data in 1D array
                //this was introduced in ES2019
                throw modalStatErrors.flat();
              }else{
                this.toastr.error(error.statusText==='OK' ? 'Unauthorized' : error.statusText, error.status);
              }
              break;
            case 401:

              /***
               * It basically comes down to the http/2 spec that removed the statusText from the response.  
               * It could be that a network device uses http/2 on your network that causes this.    
               * I am not sure why Angular chooses to populate this field with "OK" by default 
               * as this is just plain weird for anything other than a 200 response.   
               * You should be able to work around this by doing something like 
               * the following in your interceptor for the 401 for example:

                this.toastr.error(error.statusText === "OK" ? "Unauthorised" : error.statusText, error.status);
               */

              // this.toastr.error(error.statusText, error.status);
              this.toastr.error(error.statusText==='OK' ? 'Unauthorized' : error.statusText, error.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl("/server-error", navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    )
  }
}
