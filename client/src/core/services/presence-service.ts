import { inject, Injectable,signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import {HubConnection, HubConnectionBuilder, HubConnectionState} from '@microsoft/signalr';
import { User } from '../../types/user';
import { email } from '@angular/forms/signals';
import { Message } from '../../types/message';


@Injectable({
  providedIn: 'root',
})
export class PresenceService {

  private hubUrl = environment.hubUrl;
  private toast = inject(ToastService);
  hubConnection?: HubConnection
  onlineUsers = signal<string[]>([]);

  createHubConnection(user: User){

    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl+ 'presence', {

accessTokenFactory:() => user.token

    })
.withAutomaticReconnect()
.build();

this.hubConnection.start()
.catch(error => console.log(error));

this.hubConnection.on('UserOnline', userId =>{

this.onlineUsers.update(users=> [...users,userId]);

//this.toast.success(email + 'has connected');

})

this.hubConnection.on('UserOffline',userId =>{

  this.onlineUsers.update(users =>users.filter(x=>x !== userId))

//this.toast.info(email + 'has disconnected')

});

this.hubConnection.on('GetOnlineUsers', userId =>{

this.onlineUsers.set(userId);

});

this.hubConnection.on('NewMessageReceived', (message:Message)=>{


this.toast.info(message.senderDisplayName + ' has sent you a new message', 10000, message.senderImageUrl,`/members/${message.senderId}/messages`);

})

  }

  stopHubConnection(){

if(this.hubConnection?.state === HubConnectionState.Connected){

this.hubConnection.stop().catch(error => console.log(error));

}

  }
  
}
