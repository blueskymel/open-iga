import { Injectable } from '@angular/core';
import { API_SCOPE } from './api.service';
import { PublicClientApplication, AccountInfo, AuthenticationResult, PopupRequest, RedirectRequest, SilentRequest } from '@azure/msal-browser';

const msalConfig = {
  auth: {
    clientId: 'a8d02588-0f74-414d-bfdd-5f5afb80d13f',
    authority: 'https://login.microsoftonline.com/41642c57-0ffc-466f-b35d-9820ecc40af6',
    redirectUri: 'http://localhost:4200'
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: false
  }
};

@Injectable({ providedIn: 'root' })
export class AuthService {
  private static _instance: PublicClientApplication;
  private static _initPromise: Promise<void>;

  constructor() {
    if (!AuthService._instance) {
      AuthService._instance = new PublicClientApplication(msalConfig);
      AuthService._initPromise = (async () => {
        await AuthService._instance.initialize();
        await AuthService._instance.handleRedirectPromise();
      })();
    }
  }

  private async ready() {
    await AuthService._initPromise;
  }

  async login(request?: PopupRequest | RedirectRequest) {
    await this.ready();
    return AuthService._instance.loginRedirect(request || { scopes: [API_SCOPE] });
  }

  async logout() {
    await this.ready();
    return AuthService._instance.logoutRedirect();
  }

  async getAccount(): Promise<AccountInfo | null> {
    await this.ready();
    const accounts = AuthService._instance.getAllAccounts();
    return accounts && accounts.length > 0 ? accounts[0] : null;
  }

  async acquireToken(scopes: string[] = [API_SCOPE]): Promise<string | null> {
    await this.ready();
    const account = await this.getAccount();
    if (!account) return null;
    try {
      const result = await AuthService._instance.acquireTokenSilent({
        account,
        scopes
      } as SilentRequest);
      return result.accessToken;
    } catch (e) {
      // fallback to interactive
      const result = await AuthService._instance.acquireTokenPopup({ scopes } as PopupRequest);
      return result.accessToken;
    }
  }
}
