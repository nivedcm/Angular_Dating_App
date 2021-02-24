import { Component, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient  } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users: any;

  constructor(private http: HttpClient){
    // setTheme('bs4');
  }

  ngOnInit() {
    this.getAllUsers();
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

