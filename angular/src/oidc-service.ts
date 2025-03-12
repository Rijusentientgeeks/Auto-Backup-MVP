import { Injectable } from '@angular/core';
import { UserManager, User, UserManagerSettings, WebStorageStateStore, UserSettings, Profile } from 'oidc-client';

@Injectable({
  providedIn: 'root'
})
export class OidcService {

  private userManager: UserManager;

  constructor() {
    debugger
    const settings: UserManagerSettings = {
      authority: 'https://login.microsoftonline.com/d6a975ac-66de-46cc-8cf4-79991832f2b7/v2.0',  // The URL of your OpenID Connect provider (e.g., Azure AD, IdentityServer)
      client_id: '68b3b0b4-7606-442e-8e0c-9396af3f2f8d',
      //redirect_uri: window.location.origin + '/app/about',
      redirect_uri: 'http://localhost:4200/callback',
      response_type: 'code',
      scope: 'openid profile email',  // Adjust based on your required scopes
      //post_logout_redirect_uri: window.location.origin,
      post_logout_redirect_uri: "http://localhost:4200/",
      automaticSilentRenew: true,
      silent_redirect_uri: "http://localhost:4200/silent-renew.html" // Silent renew (optional)
    };

    this.userManager = new UserManager(settings);
  }

  public login() {
    debugger
    this.userManager.signinRedirect();
  }

  public logout() {
    debugger
    this.userManager.signoutRedirect();
  }

  public getUser(): Promise<User | null> {
    debugger
    // Get the URL hash (the part after the # symbol)
    // const urlHash = window.location.hash;

    // // Find the id_token in the URL hash
    // const params = new URLSearchParams(urlHash.slice(1)); // Remove the # symbol
    // const idToken = params.get('id_token');
    // if (idToken != null) {
    //   console.log(idToken); // This will log the token to the console
    //   // Example JWT token (you can replace this with your actual token)
    //   const token = idToken;

    //   // Split the token into its three parts
    //   const parts = token.split('.');

    //   // Decode the payload (second part) from Base64URL to Base64
    //   const base64Url = parts[1];
    //   const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    //   const decodedPayload = JSON.parse(atob(base64));

    //   // Now we have the payload, and we can extract the email
    //   const email = decodedPayload.email;
    //   console.log(email); // This will print the email from the token

    //   const userSettings: UserSettings = {
    //     id_token: idToken,
    //     access_token: idToken,
    //     session_state: params.get('session_state') || undefined,
    //     refresh_token: base64,
    //     token_type: 'id_token',
    //     scope: 'openid profile email',
    //     profile: null,
    //     expires_at: decodedPayload.exp,
    //     state: decodedPayload.state
    //   };
  
    //   const user = new User(userSettings); // Create User instance
  
    //   return Promise.resolve(user);
    // }
    return this.userManager.getUser();
  }

  public handleCallback() {
    return this.userManager.signinRedirectCallback();
  }

  public isAuthenticated(): Promise<boolean> {
    debugger
    return this.getUser().then(user => user != null);
  }
}
