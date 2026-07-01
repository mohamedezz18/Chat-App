import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../Environments/environment.development';
import { tap, map } from 'rxjs';
import { User, UserLogin, UserRegister } from '../../Models/User';
import { LikesService } from './likes-service';
import { PresenceService } from './presence-service';
import { HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private _likesService = inject(LikesService);
  private presenceService = inject(PresenceService);
  public currentUser = signal<User | null>(null);
  private baseUrl = environment.BaseUrl;

  register(model: UserRegister) {
    return this.http
      .post<User>(`${this.baseUrl}account/register`, model, { withCredentials: true })
      .pipe(
        tap((User) => {
          if (User) {
            this.setCurrentUser(User);
            this.startTokenRefreshInterval();
          }
        }),
      );
  }

  login(model: UserLogin) {
    return this.http
      .post<User>(`${this.baseUrl}account/login`, model, { withCredentials: true })
      .pipe(
        tap((User) => {
          if (User) {
            this.setCurrentUser(User);
            this.startTokenRefreshInterval();
          }
        }),
      );
  }

  refreshToken() {
    return this.http.post<User>(
      `${this.baseUrl}account/refresh-token`,
      {},
      {
        withCredentials: true,
      },
    );
  }

  startTokenRefreshInterval() {
    setInterval(
      () => {
        this.http
          .post<User>(`${this.baseUrl}account/refresh-token`, {}, { withCredentials: true })
          .subscribe({
            next: (user) => {
              this.setCurrentUser(user);
            },
            error: () => {
              this.logout();
            },
          });
      },
      10 * 24 * 60 * 60 * 1000,
    );
  }

  logout() {
    this.http.post(`${this.baseUrl}account/logout`, {}, { withCredentials: true }).subscribe({
      next: () => {
        localStorage.removeItem('filters');
        this._likesService.clearLikeIds();
        this.currentUser.set(null);
        this.presenceService.stopHubConnection();
      },
    });
  }

  setCurrentUser(user: User) {
    user.roles = this.getRolesFromToken(user);
    this.currentUser.set(user);
    this._likesService.getLIkeIds();
    if (this.presenceService.hubConnection?.state !== HubConnectionState.Connected) {
      this.presenceService.createHubConnection(user);
    }
  }

  private getRolesFromToken(user: User) {
    const payload = user.token.split('.')[1];
    const decoded = atob(payload);
    const jsonPayload = JSON.parse(decoded);
    return Array.isArray(jsonPayload.roles) ? jsonPayload.roles : [jsonPayload.roles];
  }
}
