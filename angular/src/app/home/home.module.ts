import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home.component';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { AutoBackupServiceProxy } from '@shared/service-proxies/service-proxies';

@NgModule({
    declarations: [HomeComponent],
    imports: [SharedModule, HomeRoutingModule,CardModule,ButtonModule,CommonModule],
    providers: [AutoBackupServiceProxy],
})
export class HomeModule {}
