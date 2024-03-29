import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { Group } from '../_models/messageGroup';
import { User } from '../_models/user';
import { BusyService } from './busy.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messagethread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient, private busyService: BusyService) { }

  createHubConnection(user: User, otherUserName: string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
        .withUrl(this.hubUrl + 'message?user=' + otherUserName, {
          accessTokenFactory: () => user.token
        })
        .withAutomaticReconnect()
        .build()

    this.hubConnection.start().catch(er => console.log(er)).finally(() => this.busyService.idle());

    this.hubConnection.on('ReceiveMessageThread', messages =>{
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('NewMessage', message =>{
      this.messagethread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message])
      });
    });

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if(group.connections.some(x => x.username == otherUserName)) {
        this.messagethread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if(!message.dateRead){
              message.dateRead = new Date(Date.now());
            }
          })
          this.messageThreadSource.next([...messages]);
        });
      }
    });
  }

  stopHubConnection() {
    if(this.hubConnection) {
      this.messageThreadSource.next([]);
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl+'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  // sendMessage(username: string, content: string) {
  //   return this.http.post<Message>(this.baseUrl + 'messages', {RecipiantUserName: username, content});
  // }

  async sendMessage(username: string, content: string) {
    try {
      return this.hubConnection.invoke('SendMessage', { RecipiantUserName: username, content });
    } catch (er) {
      return console.log(er);
    }
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/'+ id);
  }
}
