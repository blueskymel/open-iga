import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from './auth.service';
import { ApiService } from './api.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="username$ | async as username; else notLoggedIn">
      <p>Logged in as: {{ username }}</p>
      <button (click)="logout()">Logout</button>
      <button (click)="callApi()">Call API</button>
      <div *ngIf="apiResult">API Result: {{ apiResult | json }}</div>
    </div>
    <ng-template #notLoggedIn>
      <p>Please log in.</p>
    </ng-template>
  `
})
export class HomeComponent {
  apiResult: any = null;
  username$: Promise<string | null>;

  constructor(private auth: AuthService, private api: ApiService) {
    this.username$ = this.auth.getAccount().then(acc => acc?.name ?? null);
  }

  callApi() {
    this.api.getUsers().subscribe({
      next: (data) => this.apiResult = data,
      error: (err) => this.apiResult = err
    });
  }

  async logout() {
    await this.auth.logout();
  }
}
