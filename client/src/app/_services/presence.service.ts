import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$= this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'presence', {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build()

    this.hubConnection
    .start()
    .catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      //this.toastr.info(username + ' is online');
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames, username])
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      //this.toastr.warning(username + ' is offline');
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames.filter(x => x !== username)])
      })
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) =>{
      this.onlineUserSource.next(usernames);
    })

    this.hubConnection.on('NewMessageReceived', (response) =>{
      debugger;
      this.toastr.info('You have a new message from ' + response.knownAs)
      .onTap.pipe(take(1))
      .subscribe(() => this.router.navigateByUrl('/members/'+ response.username +'?tab=3'))

    })
  }

  stopHubConnection() {
    this.hubConnection.stop().catch(er=> console.log(er));
  }
}
