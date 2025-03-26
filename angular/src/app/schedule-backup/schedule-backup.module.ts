import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ScheduleBackupRoutingModule } from './schedule-backup-routing.module';
import { ScheduleBackupComponent } from './schedule-backup.component';
import { DropdownModule } from 'primeng/dropdown';
import { ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SourceConfiguationServiceProxy } from '@shared/service-proxies/service-proxies';


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
  ],
  exports: [ScheduleBackupComponent],
  providers: [SourceConfiguationServiceProxy]
})
export class ScheduleBackupModule { }
