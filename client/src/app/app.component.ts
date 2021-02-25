import { Component, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient  } from '@angular/common/http';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users: any;

  constructor(private http: HttpClient, private accountService : AccountService){
  }

  ngOnInit() {
    this.getAllUsers();
    this.setCurrentUser();
  }

  setCurrentUser(){
    const user: User = JSON.parse(localStorage.getItem('user'));
    this.accountService.setCurrentUser(user);
  }

  getAllUsers() {

    var url = "https://localhost:44364/api/users";

    this.http.get(url).subscribe(response =>{
      this.users = response;
    }, error =>{
      console.log(error);
    })
  }
}
