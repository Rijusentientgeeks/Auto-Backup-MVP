import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AppComponent } from './app.component';
import { CallbackComponent } from './callback/callback.component'; // Create a component to handle the callback
import { LoginComponent } from 'account/login/login.component';
import { AboutComponent } from './about/about.component';
import { OidcService } from 'oidc-service';

// const routes: Routes = [
//     { path: 'callback', component: CallbackComponent }
// ];

// @NgModule({
//     imports: [
//         RouterModule.forChild([
//             {
//                 path: '',
//                 component: AppComponent,
//                 children: [
//                     {
//                         path: 'home',
//                         loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
//                         canActivate: [AppRouteGuard]
//                     },
//                     {
//                         path: 'about',
//                         loadChildren: () => import('./about/about.module').then((m) => m.AboutModule),
//                         canActivate: [AppRouteGuard]
//                     },
//                     {
//                         path: 'users',
//                         loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
//                         data: { permission: 'Pages.Users' },
//                         canActivate: [AppRouteGuard]
//                     },
//                     {
//                         path: 'roles',
//                         loadChildren: () => import('./roles/roles.module').then((m) => m.RolesModule),
//                         data: { permission: 'Pages.Roles' },
//                         canActivate: [AppRouteGuard]
//                     },
//                     {
//                         path: 'tenants',
//                         loadChildren: () => import('./tenants/tenants.module').then((m) => m.TenantsModule),
//                         data: { permission: 'Pages.Tenants' },
//                         canActivate: [AppRouteGuard]
//                     },
//                     {
//                         path: 'update-password',
//                         loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
//                         canActivate: [AppRouteGuard]
//                     },
//                 ]
//             }
//         ])
//     ],
//     exports: [RouterModule]

//     // imports: [RouterModule.forRoot(routes)],
//     // exports: [RouterModule]
// })
// export class AppRoutingModule { }

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'callback', component: CallbackComponent }, // After OIDC login redirect
    // { path: 'logout', component: LogoutComponent },
    { path: '', component: AboutComponent },
  ];
  
  @NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
  })
  export class AppRoutingModule {
    constructor(private authService: OidcService) {
      this.authService.handleCallback().then(user => {
        if (user) {
          console.log("User logged in", user);
        } else {
          console.log("User not logged in");
        }
      });
    }
  }