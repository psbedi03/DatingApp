import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}

  constructor(public accountService: AccountService, 
    private router: Router, 
    private toastr: ToastrService) {}

  ngOnInit(): void {
  }

  login(){
    console.log("data entered in components");
    console.log(this.model);
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
        this.router.navigateByUrl('/members');
      }
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  // getCurrentUser(){
  //   //currentUser$ is onservable, so we have to subscribe
  //   this.accountService.currentUser$.subscribe(user => {
  //     this.loggedIn = !!user; //!! will check if the user is null then !!user = false
  //   }, error => {
  //     console.log("getCurrentUser error: "+error);
  //   })
  // }

}
