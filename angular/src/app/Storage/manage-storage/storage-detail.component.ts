import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
} from "@angular/core";

@Component({
  selector: "app-storage-detail",

  templateUrl: "./storage-detail.component.html",
  styleUrl: "./storage-detail.component.css",
})
export class StorageDetailComponent implements OnChanges {
  @Input() entries: any[] = [];
  @Input() selectedStorage: string = "";
  @Output() back = new EventEmitter<void>();
  filteredEntries: any[] = [];

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
  // Dynamically get keys that are not null or undefined
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
      awS_AccessKey: 'Access Key',
      awS_SecretKey: 'Secret Key',
      awS_BucketName: 'Bucket Name',
      awS_Region: 'Region',
      awS_backUpPath: 'Backup Path',
      aZ_AccountName:'Account Name',
      aZ_AccountKey:'Account key',
      nfS_IP:'IP Address',
      nfS_AccessUserID:'Access user ID',
      nfS_Password:'Password',
      nfS_LocationPath:'Location Path'
    };
  
    return labelMap[key] || key
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, (str) => str.toUpperCase())
      .replace('Nf S', 'Network File System')
      .replace('Aw S', 'AWS')
      .replace('Az', 'Azure');
  }
  goBack() {
    this.back.emit();
  }
}
