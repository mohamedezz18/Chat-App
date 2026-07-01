import { Component, inject, output, signal } from '@angular/core';
import { Register } from '../account/register/register';
import { AccountService } from '../../Core/Services/account-service';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  protected registerMode = signal(false);
  protected AccountService = inject(AccountService);
  ShowRegisterMode() {
    this.registerMode.set(true);
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode.set(event);
  }
}
