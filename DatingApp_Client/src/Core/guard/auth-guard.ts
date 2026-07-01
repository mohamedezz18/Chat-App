import { CanActivateFn } from '@angular/router';
import { ToastService } from '../Services/toast-service';
import { AccountService } from '../Services/account-service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toast = inject(ToastService);

  if (accountService.currentUser()) return true;
  else {
    toast.error('You shall not pass');
    return false;
  }
};
