import { Injectable, inject } from '@angular/core';
import { tap } from 'rxjs';
import { AccountService } from '../Services/account-service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private accountService = inject(AccountService);

  init() {
    return this.accountService.refreshToken().pipe(
      tap((user) => {
        if (user) {
          this.accountService.setCurrentUser(user);
          this.accountService.startTokenRefreshInterval();
        }
      }),
    );
  }
}
