import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { SourceConfigurationRoutingModule } from "./source-configuration-routing.module";
import { SourceConfigurationComponent } from "./source-configuration.component";
import { DialogModule } from "primeng/dialog";
import { ReactiveFormsModule } from "@angular/forms";
import { ButtonModule } from "primeng/button";
import { DropdownModule } from "primeng/dropdown";
import { InputTextModule } from "primeng/inputtext";
import { BackUpStorageConfiguationServiceProxy, BackUPTypeServiceProxy, DBTypeServiceProxy, SourceConfiguationServiceProxy } from "@shared/service-proxies/service-proxies";
import { FileUploadModule } from 'primeng/fileupload';
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
    FileUploadModule
    
    
  ],
  providers: [BackUPTypeServiceProxy,DBTypeServiceProxy,BackUpStorageConfiguationServiceProxy,SourceConfiguationServiceProxy ],
  exports: [SourceConfigurationComponent],
})
export class SourceConfigurationModule {}
