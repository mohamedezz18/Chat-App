import { AsyncPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../Core/Services/member-service';
import { Member } from '../../../Models/Member';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter, Observable } from 'rxjs';
import { AgePipe } from '../../../Core/Pipes/age-pipe';
import { AccountService } from '../../../Core/Services/account-service';
import { PresenceService } from '../../../Core/Services/presence-service';
import { LikesService } from '../../../Core/Services/likes-service';
import { HasRole } from '../../../Core/directives/has-role';

@Component({
  selector: 'app-member-detailed',
  imports: [RouterLink, RouterOutlet, AgePipe, HasRole],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css',
})
export class MemberDetailed implements OnInit {
  private _AccountService = inject(AccountService);
  protected _MemberService = inject(MemberService);
  private _ActivatedRoute = inject(ActivatedRoute);
  protected LikesService = inject(LikesService);
  protected presenceService = inject(PresenceService);
  protected Title = signal<string | undefined>('Profile');
  private RouteID = signal<string | null>('null');
  protected isCurrentUser = computed(
    () => this._AccountService.currentUser()?.id === this.RouteID(),
  );
  protected hasLiked = computed(() => this.LikesService.likeIds().includes(this.RouteID()!));
  constructor() {
    this._ActivatedRoute.paramMap.subscribe({
      next: (params) => {
        this.RouteID.set(params.get('id'));
      },
    });
  }

  ngOnInit(): void {
    // this._ActivatedRoute.data.subscribe({
    //   next: (data) => {
    //     this.member.set(data['member']);
    //   },
    // });
    this.Title.set(this._ActivatedRoute.firstChild?.snapshot?.title);

    // this._Router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe({
    //   next: () => {
    //     this.Title.set(this._ActivatedRoute.firstChild?.snapshot?.title);
    //   },
    // });
  }
}
