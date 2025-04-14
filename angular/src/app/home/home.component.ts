import {
  Component,
  Injector,
  ChangeDetectionStrategy,
  OnInit,
  ChangeDetectorRef,
} from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AutoBackupServiceProxy } from "@shared/service-proxies/service-proxies";

@Component({
  templateUrl: "./home.component.html",

  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    private cdr: ChangeDetectorRef,
    private autoBackupService: AutoBackupServiceProxy
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.loadDashboard();
  }
  metrics: any[] = [];
  lastBackup: any = {};
  nextSchedules: any[] = [];
 
  loadDashboard() {
    this.autoBackupService.getDashBoardItem().subscribe({
      next: (response) => {
        if (response) {
          const data = response;  
          this.metrics = [
            { label: 'Backup Sources', value: data.backupSourceCount, icon: 'pi pi-home', color: '#3B82F6' },
            { label: 'Backup Storages', value: data.backupStorageCount, icon: 'pi pi-database', color: '#10B981' },
            { label: 'Scheduled Backups', value: data.scheduleBackupCount, icon: 'pi pi-clock', color: '#F59E0B' },
            { label: 'Total Backups', value: data.totalBackupCount, icon: 'pi pi-check-circle', color: '#8B5CF6' },
          ];  
          this.lastBackup = {
            status: data.lastBackupStatus || 'N/A',
            item: data.lastBackupItem || 'N/A',
          };  
          this.nextSchedules = data.nextScheduleList ?? [];  
          this.cdr.detectChanges();
        }
      }
    });
  }
}
