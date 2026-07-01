import { Routes } from '@angular/router';
import { Home } from '../Features/home/home';
import { MemberList } from '../Features/members/member-list/member-list';
import { MemberDetailed } from '../Features/members/member-detailed/member-detailed';
import { Lists } from '../Features/lists/lists';
import { Messages } from '../Features/messages/messages';
import { Nav } from '../Layout/nav/nav';
import { authGuard } from '../Core/guard/auth-guard';
import { TestErrors } from '../Features/test-errors/test-errors';
import { NotFound } from '../Shared/errors/not-found/not-found';
import { ServerError } from '../Shared/errors/server-error/server-error';
import { MemberProfile } from '../Features/members/member-profile/member-profile';
import { MemberPhoto } from '../Features/members/member-photo/member-photo';
import { MemberMessage } from '../Features/members/member-message/member-message';
import { memberResolver } from '../Features/members/member-resolver';
import { preventUnsavedChangesGuardGuard } from '../Core/guard/prevent-unsaved-changes-guard-guard';
import { Admin } from '../Features/admin/admin';
import { adminGuardGuard } from '../Core/guard/admin-guard-guard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'errors', component: TestErrors },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'members', component: MemberList },
      {
        path: 'members/:id',
        component: MemberDetailed,
        resolve: { member: memberResolver },
        runGuardsAndResolvers: 'always',
        children: [
          { path: '', redirectTo: 'profile', pathMatch: 'full' },
          {
            path: 'profile',
            component: MemberProfile,
            title: 'Profile',
            canDeactivate: [preventUnsavedChangesGuardGuard],
          },
          { path: 'photos', component: MemberPhoto, title: 'Photo' },
          { path: 'messages', component: MemberMessage, title: 'Messages' },
        ],
      },
      { path: 'lists', component: Lists },
      { path: 'messages', component: Messages },
      { path: 'admin', component: Admin, canActivate: [adminGuardGuard] },
    ],
  },
  { path: '**', component: Home },
  { path: 'not-found', component: NotFound },
  { path: 'server-error', component: ServerError },
];
