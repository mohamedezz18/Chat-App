import { CanActivateFn } from '@angular/router';
import { AccountService } from '../Services/account-service';
import { inject } from '@angular/core';
import { ToastService } from '../Services/toast-service';

export const adminGuardGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toast = inject(ToastService);

  if (
    accountService.currentUser()?.roles?.includes('Admin') ||
    accountService.currentUser()?.roles?.includes('Moderator')
  ) {
    return true;
  } else {
    toast.error('Enter this area, you cannot');
    return false;
  }
};
