import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { NgxGalleryImage } from '@kolkov/ngx-gallery';
import { NgxGalleryAnimation } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit , OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];
  user: User;


  constructor(public presence: PresenceService,
     private messageService: MessageService,
     private route: ActivatedRoute,
     private accountService: AccountService) {
       this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
     }

  ngOnInit(): void {

    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    this.route.queryParams.subscribe(params => {
      params.tab? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [
      {
        width: '600px',
        height: '400px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      },
    ];
    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls=[];
    for(const photo of this.member.photos){
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imageUrls;
  }

  // loadMember() {
  //   this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member =>{
  //       this.member = member;
  //   })
  // }

  loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    });
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if(this.activeTab.heading ==='Messages' && this.messages.length === 0) {
      //this.loadMessages();
      this.messageService.createHubConnection(this.user,this.member.username)
    }
    else{
      this.messageService.stopHubConnection();
    }
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
