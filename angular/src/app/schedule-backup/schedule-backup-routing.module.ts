import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleBackupComponent } from './schedule-backup.component';

const routes: Routes = [
      {
        path: "",
        component: ScheduleBackupComponent,
      },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleBackupRoutingModule { }
