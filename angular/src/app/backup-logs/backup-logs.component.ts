import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AutoBackupServiceProxy, BackUpLogDto, BackUPTypeDto, BackUPTypeServiceProxy, CloudStorageServiceProxy, SourceConfiguationDto, SourceConfiguationServiceProxy, StorageMasterTypeServiceProxy } from '@shared/service-proxies/service-proxies';
import { BackupService } from '@shared/service-proxies/backup-download.service';
import { LazyLoadEvent } from 'primeng/api';
@Component({
  selector: 'app-backup-log',
  templateUrl: './backup-logs.component.html',
  styleUrls: ['./backup-logs.component.css']
})
export class BackupLogsComponent implements OnInit {
  backupLogs: any[] = [];
  backupTypes: any[] = [];
  storageTypes: any[] = [];
  cloudStorages: any[] = [];
  backupConfigs: any[] = [];
  
  keyword: string = '';
  selectedStorageType: string = '';
  selectedBackupType: string = '';
  selectedCloudStorage: string = '';
  selectedSourceConfig: string = '';
  
  totalRecords: number = 0;
  loading: boolean = true;
  rows: number = 10;
  first: number = 0;

  private searchTimer: any;

  constructor(
    private backupService: AutoBackupServiceProxy,
    private backUPTypeService: BackUPTypeServiceProxy,
    private storageMasterTypeService: StorageMasterTypeServiceProxy,
    private cloudStorageService: CloudStorageServiceProxy,
    private sourceConfigService: SourceConfiguationServiceProxy,
    private backupDownloadService: BackupService,
    private cdr: ChangeDetectorRef
  ) {}

  // Remove the setTimeout and modify ngOnInit like this:
ngOnInit(): void {
  this.LoadBackupTypes();
  this.loadStorageTypes();
  this.loadCloudStorageTypes();
  this.loadSourceConfigs();
  // this.loadBackupLogsLazy({ first: 0, rows: 10 });
}


  LoadBackupTypes(): void {
      this.backUPTypeService.getAll().subscribe({
        next: (result) => {
          if (result && result.items) {
            this.backupTypes = result.items.map(
              (item: BackUPTypeDto) => ({
                name: item.name,
                value: item.id,
              })
            );
          }
        },
        error: (err) => {
          console.error("Error fetching Details:", err);
        },
      });
    }

  loadStorageTypes(): void {
    this.storageMasterTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          this.storageTypes = result.items.map(item => ({ name: item.name, value: item.id }));
        }
      },
      error: (err) => console.error('Error fetching storage types:', err)
    });
  }

  loadCloudStorageTypes(): void {
    this.cloudStorageService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          this.cloudStorages = result.items.map(item => ({ name: item.name, value: item.id }));
        }
      },
      error: (err) => console.error('Error fetching cloud storage types:', err)
    });
  }

  loadSourceConfigs(): void {
    this.sourceConfigService.getAll(undefined, undefined, 1000, 0).subscribe({
      next: (result) => {
        if (result && result.items) {
          this.backupConfigs = result.items.map(config => ({ backupName: config.backupName, backUpStorageConfiguationId: config.backUpStorageConfiguationId }));
          this.cdr.detectChanges();
        }
      },
      error: (err) => console.error('Error fetching Source Configurations:', err)
    });
  }

  onFilterChange(): void {
    if (this.searchTimer) {
      clearTimeout(this.searchTimer);
    }
    
    this.searchTimer = setTimeout(() => {
      this.first = 0;
      this.loadBackupLogsLazy({ first: 0, rows: this.rows });
    }, 500);
  }

  loadBackupLogsLazy(event: LazyLoadEvent): void {
    this.loading = true;
    
    // Ensure we have proper pagination values
    const first = event.first || 0;
    const rows = event.rows || 10;
    
    this.backupService.getAllBackupLog(
      this.keyword, 
      this.selectedSourceConfig, 
      this.selectedStorageType, 
      this.selectedBackupType, 
      this.selectedCloudStorage, 
      first, 
      rows
    ).subscribe({
      next: (result) => {
        this.backupLogs = result.items || [];
        this.totalRecords = result.totalCount || 0;
        this.loading = false;
        this.first = first; // Update the first position
        this.rows = rows;   // Update the rows per page
        this.cdr.detectChanges(); // Ensure change detection runs
      },
      error: (err) => {
        console.error('Error fetching backup logs:', err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  downloadBackup(backupLog: BackUpLogDto) {
    debugger;
    // this.backupDownloadService.downloadBackup(backupLog.sourceConfiguationId, backupLog.backUpFileName);
    this.backupDownloadService.downloadBackup({
      sourceConfigurationId: backupLog.sourceConfiguationId,
      backUpFileName: backupLog.backUpFileName,
    });
    
  }
}