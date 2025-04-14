import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  templateUrl: './home.component.html',

  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
  metrics = [
    {
      label: 'Backup Sources',
      value: 2,
      icon: 'pi pi-database',
      color: '#3B82F6'
    },
    {
      label: 'Backup Storages',
      value: 4,
      icon: 'pi pi-hdd',
      color: '#10B981'
    },
    {
      label: 'Scheduled Backups',
      value: 20,
      icon: 'pi pi-clock',
      color: '#F59E0B'
    },
    {
      label: 'Total Backups',
      value: 56,
      icon: 'pi pi-check-circle',
      color: '#8B5CF6'
    }
  ];
  
  lastBackup = {
    status: 'Success',
    item: 'Backup - DataBase - 184.174.33.208'
  };
  
  nextSchedules = [
    { name: 'Backup - DataBase - 184.174.33.208', cronExo: null },
    { name: 'Backup - DataBase - 184.174.33.208', cronExo: 'At 12:00 AM' },
    { name: 'Backup - DataBase - 184.174.33.208', cronExo: 'At 12:00 AM, only on Sunday' },
    // ... remaining entries from API
  ];
  
}
