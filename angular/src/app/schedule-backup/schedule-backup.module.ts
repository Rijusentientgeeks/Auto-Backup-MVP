import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ScheduleBackupRoutingModule } from './schedule-backup-routing.module';
import { ScheduleBackupComponent } from './schedule-backup.component';


@NgModule({
  declarations: [ScheduleBackupComponent],
  imports: [
    CommonModule,
    ScheduleBackupRoutingModule
  ]
})
export class ScheduleBackupModule { }
