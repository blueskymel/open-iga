import { importProvidersFrom } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { MsalModule, MsalService, MSAL_INSTANCE } from '@azure/msal-angular';
import { PublicClientApplication, InteractionType } from '@azure/msal-browser';
import { appConfig } from './app.config';
import { AuthInterceptor } from './auth.interceptor';

export function MSALInstanceFactory() {
  return new PublicClientApplication({
    auth: {
      clientId: 'a8d02588-0f74-414d-bfdd-5f5afb80d13f', // Azure Entra ID App reg clientId
      authority: 'https://login.microsoftonline.com/41642c57-0ffc-466f-b35d-9820ecc40af6', // Azure Entra ID tenant ID
      redirectUri: '/',
    },
    cache: {
      cacheLocation: 'localStorage',
    },
  });
}

export const providers = [
  importProvidersFrom(BrowserModule, MsalModule),
  provideHttpClient(withInterceptors([AuthInterceptor])),
  { provide: MSAL_INSTANCE, useFactory: MSALInstanceFactory },
  MsalService,
];

export * from './app.config';
