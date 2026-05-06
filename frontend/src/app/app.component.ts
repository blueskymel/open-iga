import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login.component';
import { HomeComponent } from './home.component';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, LoginComponent, HomeComponent],
  template: `
    <ng-container *ngIf="isLoggedIn; else showLogin">
      <app-home></app-home>
    </ng-container>
    <ng-template #showLogin>
      <app-login></app-login>
    </ng-template>
  `,
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  isLoggedIn = false;

  constructor(private auth: AuthService) {}

  async ngOnInit() {
  const token = await this.auth.acquireToken();
  console.log('TOKEN:', token);
      this.isLoggedIn = !!(await this.auth.getAccount());
  }

  async login() {
    await this.auth.login();
    // After login redirect, MSAL will reload the app and ngOnInit will re-check login state
  }

  async logout() {
    await this.auth.logout();
    this.isLoggedIn = false;
  }
}
