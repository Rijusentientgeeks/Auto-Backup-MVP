import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { TableModule } from "primeng/table";
import { BackupLogsRoutingModule } from "./backup-logs-routing.module";
import { BackupLogsComponent } from "./backup-logs.component";
import {
  AutoBackupServiceProxy,
  BackUPTypeServiceProxy,
  CloudStorageServiceProxy,
  SourceConfiguationServiceProxy,
  StorageMasterTypeServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { FormsModule } from "@angular/forms";
import { DropdownModule } from "primeng/dropdown";
import { ReactiveFormsModule } from "@angular/forms";
import { TagModule } from "primeng/tag";
import { ButtonModule } from "primeng/button";
import { ToastModule } from "primeng/toast";

@NgModule({
  declarations: [BackupLogsComponent],
  imports: [
    CommonModule,
    BackupLogsRoutingModule,
    DropdownModule,
    FormsModule,
    TableModule,
    ReactiveFormsModule,
    TagModule,
    ButtonModule,
    ToastModule,
  ],
  providers: [
    AutoBackupServiceProxy,
    BackUPTypeServiceProxy,
    StorageMasterTypeServiceProxy,
    CloudStorageServiceProxy,
    SourceConfiguationServiceProxy,
  ],
})
export class BackupLogsModule {}
