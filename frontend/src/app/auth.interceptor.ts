
import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { MsalService } from '@azure/msal-angular';
import { catchError, from, switchMap, throwError, of } from 'rxjs';
import { API_SCOPE } from './api.service';

export const AuthInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const msalService = inject(MsalService);
  const account = msalService.instance.getActiveAccount() || msalService.instance.getAllAccounts()[0];

  if (!account) {
    // Not authenticated, proceed without token
    return next(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          alert('Unauthorized. Please login again.');
        }
        return throwError(() => error);
      })
    );
  }

  // Use acquireTokenSilent to get a valid access token
  return from(msalService.acquireTokenSilent({
    account,
    scopes: [API_SCOPE] 
  })).pipe(
    switchMap((result: any) => {
      const token = result.accessToken;
      const authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
      return next(authReq);
    }),
    catchError((error: any) => {
      // If token acquisition fails, proceed without token or handle error
      return next(req);
    })
  );
};
