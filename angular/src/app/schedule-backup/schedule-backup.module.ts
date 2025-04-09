import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { ScheduleBackupRoutingModule } from "./schedule-backup-routing.module";
import { ScheduleBackupComponent } from "./schedule-backup.component";
import { DropdownModule } from "primeng/dropdown";
import { ReactiveFormsModule } from "@angular/forms";
import { ButtonModule } from "primeng/button";
import { DialogModule } from "primeng/dialog";
import { InputTextModule } from "primeng/inputtext";
import {
  BackUpFrequencyServiceProxy,
  BackUpScheduleServiceProxy,
  SourceConfiguationServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { MessageService } from "primeng/api";
import { ToastModule } from "primeng/toast";

@NgModule({
  declarations: [ScheduleBackupComponent],
  imports: [
    CommonModule,
    ScheduleBackupRoutingModule,
    DropdownModule,
    DialogModule,
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
    ToastModule,
  ],
  exports: [ScheduleBackupComponent],
  providers: [
    SourceConfiguationServiceProxy,
    BackUpFrequencyServiceProxy,
    BackUpScheduleServiceProxy,
    MessageService,
  ],
})
export class ScheduleBackupModule {}
