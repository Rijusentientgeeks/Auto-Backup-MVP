import { Component, OnInit } from '@angular/core';
import { Router } from '@node_modules/@angular/router';
import { OidcService } from 'oidc-service';

@Component({
  selector: 'app-callback',
  template: `<div>Logging in...</div>`,
})
export class CallbackComponent implements OnInit {
  constructor(private oauthService: OidcService, private router: Router) {}

  ngOnInit() {
    debugger
    // this.oauthService.tryLogin().then(() => {
    //   // You can now access user data or tokens
    //   console.log('Logged in:', this.oauthService.getIdentityClaims());
    // });
    this.oauthService.handleCallback().then(user => {
      if (user) {
        console.log("User logged in", user);
        this.router.navigate(['/']);
      } else {
        console.error("Authentication failed");
      }
    });
  }
}
