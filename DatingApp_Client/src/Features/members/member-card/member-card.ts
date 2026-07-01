import { Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Member } from '../../../Models/Member';
import { AgePipe } from '../../../Core/Pipes/age-pipe';
import { LikesService } from '../../../Core/Services/likes-service';
import { PresenceService } from '../../../Core/Services/presence-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css',
})
export class MemberCard {
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService);
  member = input.required<Member>();
  protected hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id));
  protected IsOnline = computed(() =>
    this.presenceService.onlineUsers().includes(this.member().id),
  );

  toggleLike(event: Event) {
    event.stopPropagation();
    this.likeService.toggleLike(this.member().id);
  }
}
