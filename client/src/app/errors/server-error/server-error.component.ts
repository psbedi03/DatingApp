import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

  error: any;

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    //we don't know if we'll have navigation info
    //for example if user refreshes then we lose all this data
    this.error = navigation?.extras?.state?.error;
   }

  ngOnInit(): void {
  }

}
