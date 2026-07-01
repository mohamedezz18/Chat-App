import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../Services/account-service';

export const addJwtInterceptor: HttpInterceptorFn = (req, next) => {
  const _accountService = inject(AccountService);
  const currentUser = _accountService.currentUser();
  if (currentUser) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${currentUser.token}`,
      },
    });
  }
  return next(req);
};
