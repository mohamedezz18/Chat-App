import { ResolveFn, Router } from '@angular/router';
import { MemberService } from '../../Core/Services/member-service';
import { inject } from '@angular/core';
import { Member } from '../../Models/Member';
import { EMPTY } from 'rxjs';

export const memberResolver: ResolveFn<Member | null> = (route, state) => {
  const _memberService = inject(MemberService);
  const router = inject(Router);
  const memberId = route.paramMap.get('id');

  if (memberId) {
    return _memberService.getMember(memberId);
  } else {
    router.navigateByUrl('/not-found');
    return null;
  }
};
