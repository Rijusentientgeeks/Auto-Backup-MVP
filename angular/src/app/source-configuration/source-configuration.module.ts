import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { SourceConfigurationRoutingModule } from "./source-configuration-routing.module";
import { SourceConfigurationComponent } from "./source-configuration.component";
import { DialogModule } from "primeng/dialog";
import { ReactiveFormsModule } from "@angular/forms";
import { ButtonModule } from "primeng/button";
import { DropdownModule } from "primeng/dropdown";
import { InputTextModule } from "primeng/inputtext";
import { AutoBackupServiceProxy, BackUpStorageConfiguationServiceProxy, BackUPTypeServiceProxy, DBTypeServiceProxy, SourceConfiguationServiceProxy } from "@shared/service-proxies/service-proxies";
import { FileUploadModule } from 'primeng/fileupload';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
@NgModule({
  declarations: [SourceConfigurationComponent],
  imports: [
    CommonModule,
    SourceConfigurationRoutingModule,
    DialogModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    ButtonModule,
    FileUploadModule,
    TooltipModule,
    ToastModule,
    ProgressSpinnerModule
  ],
  providers: [MessageService,BackUPTypeServiceProxy,DBTypeServiceProxy,BackUpStorageConfiguationServiceProxy,SourceConfiguationServiceProxy,AutoBackupServiceProxy ],
  exports: [SourceConfigurationComponent],
})
export class SourceConfigurationModule {}
