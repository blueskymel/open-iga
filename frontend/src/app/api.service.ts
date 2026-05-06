export const API_SCOPE = 'api://12f5d1f6-6fa3-4ba0-a8ef-befdcac7472b/access_as_user';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, from, throwError } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';

import { AuthService } from './auth.service';
import { environment } from './environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient, private auth: AuthService) {}

  getUsers(): Observable<any[]> {
    return from(this.getValidToken())
      .pipe(
        switchMap(token => {
          const headers = token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : undefined;
          return this.http.get<any[]>(`${environment.apiUrl}/api/users`, { headers });
        }),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            alert('Unauthorized. Please log in again.');
          }
          return throwError(() => error);
        })
      );
  }

  getAccessRequests(): Observable<any[]> {
    return from(this.getValidToken())
      .pipe(
        switchMap(token => {
          const headers = token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : undefined;
          return this.http.get<any[]>(`${environment.apiUrl}/api/access-requests`, { headers });
        }),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            alert('Unauthorized. Please log in again.');
          }
          return throwError(() => error);
        })
      );
  }

  // Always acquire token silently, fallback to interactive if needed
  private async getValidToken(): Promise<string | null> {
    try {
      return await this.auth.acquireToken([API_SCOPE]);
    } catch {
      // If silent fails, fallback to interactive
      return await this.auth.acquireToken([API_SCOPE]);
    }
  }
}
