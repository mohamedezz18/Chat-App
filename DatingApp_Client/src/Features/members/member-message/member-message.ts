import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  effect,
  inject,
  model,
} from '@angular/core';
import { MessageService } from '../../../Core/Services/message-service';
import { MemberService } from '../../../Core/Services/member-service';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../Core/Pipes/timeAgo-pipe';
import { PresenceService } from '../../../Core/Services/presence-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-message',
  imports: [FormsModule, DatePipe, TimeAgoPipe],
  templateUrl: './member-message.html',
  styleUrl: './member-message.css',
})
export class MemberMessage implements OnInit, OnDestroy {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;
  protected MessageService = inject(MessageService);
  protected MemberService = inject(MemberService);
  protected messageContent = model('');
  protected presenceService = inject(PresenceService);
  private _activeRoute = inject(ActivatedRoute);
  constructor() {
    effect(() => {
      this.MessageService.messageThread();
      this.scrollToBottom();
    });
  }

  ngOnInit(): void {
    this._activeRoute.parent?.params.subscribe({
      next: (params) => {
        const OtherMemberId = params['id'];
        if (!OtherMemberId) throw new Error('Member ID is required');

        void this.MessageService.createHubConnection(OtherMemberId);
      },
    });
  }

  ngOnDestroy(): void {
    void this.MessageService.stopHubConnection();
  }

  async sendMessage() {
    const recipientId = this.MemberService.member()?.id;
    if (!recipientId || !this.messageContent()) return;

    await this.MessageService.sendMessage(recipientId, this.messageContent())?.then(() => {
      this.messageContent.set('');
    });
  }

  scrollToBottom() {
    setTimeout(() => {
      if (this.messageEndRef) {
        this.messageEndRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
      }
    });
  }
}
