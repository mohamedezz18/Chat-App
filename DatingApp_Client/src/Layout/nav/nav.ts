import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../Core/Services/account-service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastService } from '../../Core/Services/toast-service';
import { themes } from '../theme';
import { LoadingService } from '../../Core/Services/loading-service';
import { HasRole } from '../../Core/directives/has-role';
import { UserLogin } from '../../Models/User';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive, HasRole],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit {
  protected _accountService = inject(AccountService);
  protected loading = inject(LoadingService);
  protected loadingSignal = signal(false);
  private _router = inject(Router);
  private _toast = inject(ToastService);
  protected _user: UserLogin = {
    email: '',
    password: '',
  };
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes;

  ngOnInit(): void {
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  }

  handleSelectTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
    const elem = document.activeElement as HTMLDivElement;
    if (elem) elem.blur();
  }

  login() {
    this.loadingSignal.set(true);
    this._accountService.login(this._user).subscribe({
      next: (response) => {
        this._router.navigateByUrl('/members');
        this._toast.success('Logged in successfully');
      },
      error: (err) => {
        this._toast.error(err.error.message);
        console.log(err);
      },
      complete: () => {
        this.loadingSignal.set(false);
      },
    });
  }

  logout() {
    this._accountService.logout();
    this._router.navigateByUrl('/');
  }
}
