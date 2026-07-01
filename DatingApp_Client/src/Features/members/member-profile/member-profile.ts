import { Component, inject, OnDestroy, OnInit, signal, ViewChild, viewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { single } from 'rxjs';
import { Member, MemberUpdate } from '../../../Models/Member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../Core/Services/member-service';
import { ToastService } from '../../../Core/Services/toast-service';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../../../Core/Services/account-service';
import { TimeAgoPipe } from '../../../Core/Pipes/timeAgo-pipe';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule, TimeAgoPipe],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css',
})
export class MemberProfile implements OnInit, OnDestroy {
  @ViewChild('editForm') editForm?: NgForm;
  protected MemberService = inject(MemberService);
  private _toast = inject(ToastService);
  private _AccountService = inject(AccountService);
  protected memberEdit: MemberUpdate = {
    displayName: '',
    city: '',
    country: '',
    description: '',
  };

  ngOnInit(): void {
    // this._ActivateRoute.parent?.data.subscribe((data) =>
    //   this.MemberService.member.set(data['member']),
    // );

    this.memberEdit = {
      displayName: this.MemberService.member()?.displayName || '',
      city: this.MemberService.member()?.city || '',
      country: this.MemberService.member()?.country || '',
      description: this.MemberService.member()?.description || '',
    };
  }

  ngOnDestroy(): void {
    if (this.MemberService.EditMode()) {
      this.MemberService.EditMode.set(false);
    }
  }

  UpdateProfile() {
    if (!this.MemberService.member()) return;
    const Update = { ...this.MemberService.member(), ...this.memberEdit };
    this.MemberService.UpdateMember(this.memberEdit).subscribe({
      next: () => {
        const currentUser = this._AccountService.currentUser();
        if (currentUser && Update.displayName !== currentUser?.displayName) {
          currentUser.displayName = Update.displayName;
          this._AccountService.setCurrentUser(currentUser);
        }
        this._toast.success('Update Success');
        this.MemberService.EditMode.set(false);
        this.MemberService.member.set(Update as Member);
        this.editForm?.reset(Update);
      },
    });
  }
}
