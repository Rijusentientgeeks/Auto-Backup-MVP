import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManageStorageRoutingModule } from './manage-storage-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ManageStorageComponent } from './manage-storage.component';
import { StorageDetailComponent } from './storage-detail.component';
import { StorageMasterTypeServiceProxy, CloudStorageServiceProxy, BackUpStorageConfiguationServiceProxy } from '@shared/service-proxies/service-proxies';


@NgModule({
  declarations: [ManageStorageComponent,StorageDetailComponent],
  imports: [
    CommonModule,
    ManageStorageRoutingModule,
      CommonModule,
        CardModule,
        ReactiveFormsModule,
        DropdownModule,
        InputTextModule,
        ButtonModule,
        PasswordModule,
        DialogModule,
    
    
  ],
  exports:[ManageStorageComponent],
  providers: [StorageMasterTypeServiceProxy, CloudStorageServiceProxy,BackUpStorageConfiguationServiceProxy],

})
export class ManageStorageModule { }
