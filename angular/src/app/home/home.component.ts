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
    { label: 'Last Backup', value: 'Success', icon: 'pi pi-check-circle', color: 'green' },
    { label: 'Next Scheduled', value: 'Today 3:00 PM', icon: 'pi pi-clock', color: 'blue' },
    { label: 'Total Backups', value: '24', icon: 'pi pi-database', color: 'orange' },
  ];

  logs = [
    { type: 'File', status: 'Success', time: '10:30 AM', trigger: 'Quick Backup' },
    { type: 'Database', status: 'Failed', time: '09:00 AM', trigger: 'Scheduled' },
    { type: 'File', status: 'Success', time: 'Yesterday', trigger: 'On-Demand' },
  ];

  chartData = {
    labels: ['Success', 'Failed', 'Pending'],
    datasets: [
      {
        data: [15, 5, 4],
        backgroundColor: ['#4caf50', '#f44336', '#ff9800'],
      },
    ],
  };

  chartOptions = {
    plugins: {
      legend: {
        position: 'bottom',
      },
    },
  };
}
