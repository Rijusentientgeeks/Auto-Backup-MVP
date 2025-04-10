import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
} from "@angular/core";
import { Router } from "@node_modules/@angular/router";
import { LazyLoadEvent } from "@node_modules/primeng/api";
import { BackupService } from "@shared/service-proxies/backup-download.service";
import {
  AutoBackupServiceProxy,
  BackUpLogDto,
  BackUpStorageConfiguationServiceProxy,
  SourceConfiguationDto,
} from "@shared/service-proxies/service-proxies";

@Component({
  selector: "app-storage-detail",

  templateUrl: "./storage-detail.component.html",
  styleUrl: "./storage-detail.component.css",
})
export class StorageDetailComponent implements OnChanges, OnInit {
  @Input() entries: any[] = [];
  @Input() selectedStorage: string = "";
  @Output() back = new EventEmitter<void>();
  @Output() editEntry = new EventEmitter<any>();
  filteredEntries: any[] = [];
  successfulBackupLogs: BackUpLogDto[] = [];
  totalSuccessfulLogs: number = 0;
  showBackupLogDialog: boolean = false;
  selectedStorageConfigurationId: string;
  downloadingIds: Set<string> = new Set();

  constructor(
    private backUpStorageConfiguationService: BackUpStorageConfiguationServiceProxy,
    private autoBackupService: AutoBackupServiceProxy,
    private backupDownloadService: BackupService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}
  ngOnInit(): void {
    var res = this.entries;
  }
  ngOnChanges(): void {
    this.filterData();
  }
  filterData(): void {
    this.filteredEntries = this.entries.filter(
      (entry) =>
        entry.storageMasterType?.name.toLowerCase() ===
        this.selectedStorage.toLowerCase()
    );
  }

  allowedKeys: string[] = [
    "awS_AccessKey",
    "awS_SecretKey",
    "awS_BucketName",
    "awS_Region",
    "awS_backUpPath",
    "aZ_AccountName",
    "aZ_AccountKey",
    "nfS_IP",
    "nfS_AccessUserID",
    "nfS_Password",
    "nfS_LocationPath",
  ];
  getFilteredKeys(entry: any): string[] {
    return Object.keys(entry).filter(
      (key) =>
        this.allowedKeys.includes(key) &&
        entry[key] !== null &&
        entry[key] !== undefined
    );
  }
  getLabel(key: string): string {
    const labelMap: { [key: string]: string } = {
      awS_AccessKey: "Access Key",
      awS_SecretKey: "Secret Key",
      awS_BucketName: "Bucket Name",
      awS_Region: "Region",
      awS_backUpPath: "Backup Path",
      aZ_AccountName: "Account Name",
      aZ_AccountKey: "Account key",
      nfS_IP: "IP Address",
      nfS_AccessUserID: "Access user ID",
      nfS_Password: "Password",
      nfS_LocationPath: "Location Path",
    };

    return (
      labelMap[key] ||
      key
        .replace(/([A-Z])/g, " $1")
        .replace(/^./, (str) => str.toUpperCase())
        .replace("Nf S", "Network File System")
        .replace("Aw S", "AWS")
        .replace("Az", "Azure")
    );
  }
  goBack() {
    this.back.emit();
  }

  GetDetailsbyID(entry: string) {
    this.backUpStorageConfiguationService.get(entry).subscribe({
      next: (result) => {
        this.editEntry.emit(result);
      },
    });
  }

  deleteEntryDetails(id: string) {
    this.backUpStorageConfiguationService.delete(id).subscribe({
      next: () => {
        this.back.emit();
      },
    });
  }

  openBackupLogDialog(id: string) {
    this.selectedStorageConfigurationId = id;
    this.showBackupLogDialog = true;

    // Manually trigger initial data load
    const initialLazyLoadEvent: LazyLoadEvent = {
      first: 0,
      rows: 10,
    };
    this.getSuccessullBackupLogLazy(initialLazyLoadEvent);
  }

  getSuccessullBackupLogLazy(event: LazyLoadEvent) {
    if (!this.showBackupLogDialog) return;

    const skipCount = event.first ?? 0;
    const maxResultCount = event.rows ?? 10;

    this.autoBackupService
      .getAllCompletedBackupLogByStorageConfigId(
        this.selectedStorageConfigurationId,
        skipCount,
        maxResultCount
      )
      .subscribe({
        next: (result) => {
          this.successfulBackupLogs = result.items || [];
          this.totalSuccessfulLogs = result.totalCount || 0;
        },
        error: (err) => {
          console.error("Error fetching backup logs:", err);
        },
      });
  }

  downloadBackup(backupLog: BackUpLogDto) {
    const key = this.getDownloadKey(backupLog);
    this.downloadingIds.add(key);
  
    this.backupDownloadService.downloadBackup$({
      sourceConfigurationId: backupLog.sourceConfiguationId,
      backUpFileName: backupLog.backUpFileName
    }).subscribe({
      next: (response) => {
        const blob = response.body!;
        const contentDisposition = response.headers.get('Content-Disposition');
        const filename = this.getFilenameFromDisposition(contentDisposition, backupLog.backUpFileName);
  
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = filename;
        link.target = '_blank';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(link.href);
      },
      error: (err) => {
        this.downloadingIds.delete(key);
        console.error('Download failed:', err);
      },
      complete: () => {
        this.downloadingIds.delete(key);
        this.cdr.detectChanges();
      }
    });
  }
  
  private getDownloadKey(backupLog: BackUpLogDto): string {
    return backupLog.id;
  }
  
  private getFilenameFromDisposition(contentDisposition: string | null, defaultName: string): string {
    if (!contentDisposition) return defaultName;
  
    const match = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
    return match && match[1]
      ? decodeURIComponent(match[1].replace(/['"]/g, ''))
      : defaultName;
  }
}
