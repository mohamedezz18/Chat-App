import { Component, inject, isStandalone, OnInit, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Nav } from '../Layout/nav/nav';
import { AccountService } from '../Core/Services/account-service';
import { Home } from '../Features/home/home';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Nav],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected router = inject(Router);
  protected readonly title = signal('DatingApp_Client');
}
