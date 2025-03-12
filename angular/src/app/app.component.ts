import { Component, Injector, OnInit, Renderer2 } from '@angular/core';
import { OAuthService } from '@node_modules/angular-oauth2-oidc';
import { AppComponentBase } from '@shared/app-component-base';
import { SignalRAspNetCoreHelper } from '@shared/helpers/SignalRAspNetCoreHelper';
import { LayoutStoreService } from '@shared/layout/layout-store.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent extends AppComponentBase implements OnInit {
  sidebarExpanded: boolean;

  constructor(
    injector: Injector,
    private renderer: Renderer2,
    private _layoutStore: LayoutStoreService,
    private oauthService: OAuthService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    debugger
    this.configureOAuth();
    this.renderer.addClass(document.body, 'sidebar-mini');

    SignalRAspNetCoreHelper.initSignalR();

    abp.event.on('abp.notifications.received', (userNotification) => {
      abp.notifications.showUiNotifyForUserNotification(userNotification);

      // Desktop notification
      Push.create('AbpZeroTemplate', {
        body: userNotification.notification.data.message,
        icon: abp.appPath + 'assets/app-logo-small.png',
        timeout: 6000,
        onClick: function () {
          window.focus();
          this.close();
        }
      });
    });

    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    });
  }

  configureOAuth() {
    debugger
    this.oauthService.configure({
      issuer: 'https://login.microsoftonline.com', // Your OpenID provider URL
      clientId: '68b3b0b4-7606-442e-8e0c-9396af3f2f8d', // Your client ID
      redirectUri: window.location.origin + '/callback', // Redirect URI after login
      scope: 'openid profile email', // Scopes you want to request
      responseType: 'id_token',
      tokenEndpoint: 'https://your-openid-provider.com/token', // Token URL
      //authorizationEndpoint: 'https://your-openid-provider.com/authorize', // Authorization URL
      userinfoEndpoint: 'https://login.microsoftonline.com/d6a975ac-66de-46cc-8cf4-79991832f2b7/oauth2/v2.0/authorize', // Authorization URL
      logoutUrl: 'https://login.microsoftonline.com/logout', // Logout URL
      postLogoutRedirectUri: window.location.origin, // Redirect after logout
    });

    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

  login() {
    debugger
    this.oauthService.initCodeFlow();
  }

  logout() {
    this.oauthService.logOut();
  }

  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
  }
}
