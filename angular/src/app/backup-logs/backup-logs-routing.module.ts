import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BackupLogsComponent } from './backup-logs.component';

const routes: Routes = [
  {
    path: '',
    component: BackupLogsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BackupLogsRoutingModule { }
