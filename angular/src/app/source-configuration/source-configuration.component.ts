import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MessageService } from "primeng/api";
import {
  AutoBackupServiceProxy,
  BackUpStorageConfiguationServiceProxy,
  BackUPTypeDto,
  BackUPTypeServiceProxy,
  DBTypeDto,
  DBTypeServiceProxy,
  SourceConfiguationCreateDto,
  SourceConfiguationDto,
  SourceConfiguationServiceProxy,
  SourceConfiguationUpdateDto,
} from "@shared/service-proxies/service-proxies";
import { LocalBackupService } from "@shared/service-proxies/local-backup.service";
import Swal from "sweetalert2";
import { formatDate } from '@angular/common';

@Component({
  selector: "app-source-configuration",
  templateUrl: "./source-configuration.component.html",
  styleUrl: "./source-configuration.component.css",
})
export class SourceConfigurationComponent implements OnInit {
  sourceConfigs: SourceConfiguationDto[] = [];
  displayDialog: boolean = false;
  sourceForm!: FormGroup;
  isEdit: boolean = false;
  selectedConfigId: string | null = null;
  BackupTypes = [];
  DbTypes = [];
  BackupStorageConfigs = [];
  showDatabaseFields: boolean = false;
  base64String: string = null;
  noData: false;
  isSaving: boolean = false;
  isLoading = false;

  osOptions = [
    { label: "Windows", value: "Windows" },
    { label: "Linux", value: "Linux" },
    { label: "MacOS", value: "MacOS" },
  ];
  nextAutoBackup: string;

  constructor(
    private fb: FormBuilder,
    private BackUPTypeService: BackUPTypeServiceProxy,
    private DatabaseTypeService: DBTypeServiceProxy,
    private BackupStorageConfigService: BackUpStorageConfiguationServiceProxy,
    private sourceConfigService: SourceConfiguationServiceProxy,
    private autoBackupService: AutoBackupServiceProxy,
    private localBackupService: LocalBackupService,
    private cdr: ChangeDetectorRef,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadSourceConfigs();
    this.initSourceForm();
    this.LoadBackupTypes();
    this.LoadDatabaseTypes();
    this.LoadBackupStorageConfigs();
  }

  initSourceForm(): void {
    this.sourceForm = this.fb.group({
      backUPType: ["", Validators.required],
      dbType: [""],
      serverIP: [""],
      dbInitialCatalog: [""],
      port: [""],
      os: [""],
      privateKeyPath: [null],
      sourcePath: [""],
      backUpInitiatedPath: [""],
      backUpStorageConfiguationId: ["", Validators.required],
      backupName: [""],
      databaseName: [""],
      dbUsername: [""],
      dbPassword: [""],
      sshUserName: [""],
      sshPassword: [""],
    });
  }
  onFileSelect(event: any): void {
    const file = event.files[0];
    if (file) {
      this.convertToBase64(file);
    }
  }

  triggerOnDemandBackup(id: string): void {
    this.isSaving = true;
    var selectedSource = this.sourceConfigs.find((s) => s.id === id);
    if(selectedSource.backUpStorageConfiguation.isUserLocalSystem) {
      this.localBackupService.takeBackupLocal$(id).subscribe({
        next: (response) => {
          this.isSaving = false;
          const blob = response.body!;
          const contentDisposition = response.headers.get('Content-Disposition');
          const filename = this.getFilenameFromDisposition(contentDisposition);
  
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
          this.isSaving = false;
          this.cdr.detectChanges();
        },
        complete: () => {
          this.isSaving = false;
          this.cdr.detectChanges();
        }
      });
    }else{
      this.autoBackupService.createBackup(id).subscribe({
        next: (result) => {
          this.isSaving = false;
          if (result) {
            Swal.fire("Success", "Backup Created Successfully", "success");
            this.cdr.detectChanges();
          }
        },
        error: () => {
          this.isSaving = false;
          Swal.fire("Error", "Something went wrong!", "error");
        },
        complete: () => {
          this.isSaving = false;
          this.cdr.detectChanges();
        }
      });
    }
  }
  isInvalid(controlName: string): boolean {
    const control = this.sourceForm.get(controlName);
    return control?.invalid && (control?.dirty || control?.touched);
  }

  private getFilenameFromDisposition(disposition: string | null): string {
  if (!disposition) return 'backup.zip';
  
  const match = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition);
  return match && match[1] ? match[1].replace(/['"]/g, '') : 'backup.zip';
}

  // Convert PEM file to Base64 string
  convertToBase64(file: File): void {
    const reader = new FileReader();

    reader.onload = () => {
      const base64String = reader.result as string;

      // Extract the Base64 part without the data URL prefix
      const base64Data = base64String.split(",")[1];
      this.sourceForm.controls["privateKeyPath"].setValue(base64Data);
    };

    reader.onerror = (error) => {};

    // Read the file as a data URL (Base64)
    reader.readAsDataURL(file);
  }

  loadSourceConfigs(): void {
    this.sourceConfigService.getAll(undefined, undefined, 1000, 0).subscribe({
      next: (result) => {
        debugger
        if (result && result.items) {
          this.sourceConfigs = result.items;
          const cronArray = result.items[0]?.scheduledCronExpression || [];
          this.nextAutoBackup = this.getNextScheduledTime(cronArray);
          this.cdr.detectChanges();
        }
      },
      error: (err) => {},
    });
  }
  getNextScheduledTime(cronExpressions: string[]): string | null {
    const now = new Date();
    let nextDate: Date | null = null;
  
    cronExpressions
      .filter(expr => expr && expr.trim() !== '')
      .forEach(expr => {
        try {
          const next = this.calculateNextRun(expr, now);
          if (next && (!nextDate || next < nextDate)) {
            nextDate = next;
          }
        } catch (err) {
          console.error(`Invalid cron expression: ${expr}`, err);
        }
      });
  
    return nextDate ? formatDate(nextDate, 'dd.MM.yyyy hh:mm a', 'en-US') : null;
  }
  
  private parseCronPart(part: string, min: number, max: number): number[] {
    if (part === '*') {
      return Array.from({ length: max - min + 1 }, (_, i) => i + min);
    }
  
    if (part.includes('/')) {
      const [range, step] = part.split('/');
      const stepVal = parseInt(step, 10);
      const baseRange = range === '*' ? [min, max] : range.split('-').map(Number);
      const [start, end] = baseRange.length === 2 ? baseRange : [min, max];
  
      return Array.from({ length: Math.floor((end - start + 1) / stepVal) }, (_, i) => start + i * stepVal);
    }
  
    return part
      .split(',')
      .flatMap(token => {
        if (token.includes('-')) {
          const [start, end] = token.split('-').map(Number);
          return Array.from({ length: end - start + 1 }, (_, i) => start + i);
        }
        return [parseInt(token, 10)];
      });
  }
  
  private calculateNextRun(cron: string, fromDate: Date): Date | null {
    const [minStr, hourStr, dayStr, monthStr, dayOfWeekStr] = cron.trim().split(/\s+/);
    const minutes = this.parseCronPart(minStr, 0, 59);
    const hours = this.parseCronPart(hourStr, 0, 23);
    const days = this.parseCronPart(dayStr, 1, 31);
    const months = this.parseCronPart(monthStr, 1, 12);
    const daysOfWeek = this.parseCronPart(dayOfWeekStr, 0, 6);
  
    for (let i = 0; i < 365; i++) {
      const date = new Date(fromDate);
      date.setSeconds(0);
      date.setMilliseconds(0);
      date.setDate(date.getDate() + i);
  
      const month = date.getMonth() + 1;
      const day = date.getDate();
      const dow = date.getDay();
  
      if (!months.includes(month)) continue;
      if (!days.includes(day)) continue;
      if (!daysOfWeek.includes(dow)) continue;
  
      for (const hour of hours) {
        for (const minute of minutes) {
          const testDate = new Date(date);
          testDate.setHours(hour, minute, 0, 0);
          if (testDate > fromDate) return testDate;
        }
      }
    }
  
    return null;
  }
  
  
  LoadBackupTypes(): void {
    this.BackUPTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          this.BackupTypes = result.items.map((item: BackUPTypeDto) => ({
            name: item.name,
            value: item.id,
          }));
        }
      },
      error: (err) => {},
    });
  }

  LoadDatabaseTypes(): void {
    this.DatabaseTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          this.DbTypes = result.items.map((item: DBTypeDto) => ({
            name: item.name,
            value: item.id,
          }));
        }
      },
      error: (err) => {},
    });
  }
  LoadBackupStorageConfigs(): void {
    const defaultOption = {
        name: "User's Local System",
        value: '00000000-0000-0000-0000-000000000000'
      };
    this.BackupStorageConfigs = [defaultOption];
    this.BackupStorageConfigService.getAll(
      undefined,
      undefined,
      1000,
      0
    ).subscribe({
      next: (result) => {
        if (result && result.items) {
          var backupStorageConfigs = result.items.map((item: any) => ({
            name: item.backupName,
            value: item.id,
          }));
          this.BackupStorageConfigs = [
            this.BackupStorageConfigs[0],
            ...backupStorageConfigs,
          ];
        }
      },
      error: (err) => {
      },
    });
  }
  onBackupTypeChange(event: any) {
    this.showDatabaseFields = event.value?.name === "DataBase";
    if (this.showDatabaseFields) {
      this.sourceForm.controls["dbType"].setValidators(Validators.required);
      this.sourceForm.controls["serverIP"].setValidators(Validators.required);
      // this.sourceForm.controls["dbInitialCatalog"].setValidators(Validators.required);
      this.sourceForm.controls["sshUserName"].setValidators(
        Validators.required
      );
      this.sourceForm.controls["sshPassword"].clearValidators();
      this.sourceForm.controls["databaseName"].setValidators(
        Validators.required
      );
      this.sourceForm.controls["dbUsername"].setValidators(Validators.required);
      this.sourceForm.controls["dbPassword"].setValidators(Validators.required);
      this.sourceForm.controls["backUPType"].setValidators(Validators.required);
      this.sourceForm.controls["backUpStorageConfiguationId"].setValidators(
        Validators.required
      );
      this.sourceForm.controls["os"].clearValidators();
      this.sourceForm.controls["sourcePath"].clearValidators();
      this.sourceForm.controls["privateKeyPath"].clearValidators();
      this.sourceForm.controls["backUpInitiatedPath"].clearValidators();
    } else {
      this.sourceForm.controls["dbType"].clearValidators();
      this.sourceForm.controls["serverIP"].setValidators(Validators.required);
      // this.sourceForm.controls["dbInitialCatalog"].clearValidators();
      this.sourceForm.controls["sshUserName"].setValidators(
        Validators.required
      );
      this.sourceForm.controls["sshPassword"].clearValidators();
      this.sourceForm.controls["databaseName"].clearValidators();
      this.sourceForm.controls["dbUsername"].clearValidators();
      this.sourceForm.controls["dbPassword"].clearValidators();
      this.sourceForm.controls["os"].setValidators(Validators.required);
      this.sourceForm.controls["privateKeyPath"].clearValidators();
      this.sourceForm.controls["backUpInitiatedPath"].clearValidators();
      this.sourceForm.controls["sourcePath"].setValidators(Validators.required);
      this.sourceForm.controls["backUPType"].setValidators(Validators.required);
      this.sourceForm.controls["backUpStorageConfiguationId"].setValidators(
        Validators.required
      );
    }

    this.sourceForm.controls["dbType"].updateValueAndValidity();
    this.sourceForm.controls["serverIP"].updateValueAndValidity();
    // this.sourceForm.controls['dbInitialCatalog'].updateValueAndValidity();
    this.sourceForm.controls["sshUserName"].updateValueAndValidity();
    this.sourceForm.controls["sshPassword"].updateValueAndValidity();
    this.sourceForm.controls["databaseName"].updateValueAndValidity();
    this.sourceForm.controls["dbUsername"].updateValueAndValidity();
    this.sourceForm.controls["dbPassword"].updateValueAndValidity();
    this.sourceForm.controls["os"].updateValueAndValidity();
    this.sourceForm.controls["sourcePath"].updateValueAndValidity();
    this.sourceForm.controls["backUpInitiatedPath"].updateValueAndValidity();
    this.sourceForm.controls["privateKeyPath"].updateValueAndValidity();
    this.sourceForm.controls["backUPType"].updateValueAndValidity();
    this.sourceForm.controls[
      "backUpStorageConfiguationId"
    ].updateValueAndValidity();
  }
  openAddSourceDialog(): void {
    this.isEdit = false;
    this.selectedConfigId = null;
    this.sourceForm.reset();
    this.displayDialog = true;
  }

  editConfig(config: any): void {
    this.isEdit = true;
    this.selectedConfigId = config.id;
    const selectedBackupType = this.BackupTypes.find(
      (type) => type.value === config.backUPType?.id
    );
    const selectedDbType = this.DbTypes.find(
      (type) => type.value === config.dbType?.id
    );

    this.sourceForm.patchValue({
      ...config,
      sourcePath: config.sourcepath,
      backUPType: selectedBackupType,
      dbType: selectedDbType ? selectedDbType.value : null,
    });

    setTimeout(() => {
      if (selectedBackupType) {
        this.onBackupTypeChange({ value: selectedBackupType });
      }
    }, 0);

    this.displayDialog = true;
  }

  private isPasswordOrPrivateKeyProvided(): boolean {
    const password = this.sourceForm.controls["sshPassword"].value;
    const privateKey = this.sourceForm.controls["privateKeyPath"].value;
    return !!password || !!privateKey;
  }

  saveSourceConfig(): void {
    if (!this.isPasswordOrPrivateKeyProvided()) {
      this.sourceForm.controls["sshPassword"].markAsTouched();
      this.messageService.add({
        severity: "error",
        summary: "Error",
        detail: "Please provide either Password or Private Key File.",
        life: 2000,
      });
      return;
    }
    if (this.sourceForm.valid) {
      this.isSaving = true;
      const formData = this.prepareFormData();
      if (this.isEdit && this.selectedConfigId) {
        this.sourceConfigService
          .update(formData as SourceConfiguationUpdateDto)
          .subscribe(
            () => {
              this.closeDialog();
              this.isSaving = false;

              this.messageService.add({
                severity: "success",
                summary: "success",
                detail: "Configuration updated successfully!",
                life: 2000,
              });
              this.loadSourceConfigs();
            },
            (error) => {
              this.isSaving = false;
              this.messageService.add({
                severity: "error",
                summary: "Error",
                detail: "Something went wrong!",
                life: 2000,
              });
            }
          );
      } else {
        this.sourceConfigService
          .create(formData as SourceConfiguationCreateDto)
          .subscribe(
            () => {
              this.closeDialog();
              this.isSaving = false;

              this.messageService.add({
                severity: "success",
                summary: "success",
                detail: "Configuration Created successfully!",
                life: 2000,
              });
              this.loadSourceConfigs();
            },
            (error) => {
              this.isSaving = false;

              this.messageService.add({
                severity: "error",
                summary: "Error",
                detail: "Something went wrong!",
                life: 2000,
              });
            }
          );
      }
    } else {
      this.sourceForm.markAllAsTouched();
    }
  }

  prepareFormData(): SourceConfiguationCreateDto | SourceConfiguationUpdateDto {
    const formData = this.sourceForm.value;
    let description = formData.backupName?.trim();
    if (!description) {
      description = `${formData.backupName || "Backup"} - ${
        formData.backUPType?.name || "Type"
      } - ${formData.serverIP || "Server"}`;
    }
    if (this.isEdit && this.selectedConfigId) {
      return new SourceConfiguationUpdateDto({
        id: this.selectedConfigId,
        backUPTypeId: formData.backUPType.value || "",
        dbTypeId: formData.dbType || undefined,
        databaseName: formData.databaseName || undefined,
        dbUsername: formData.dbUsername || undefined,
        dbPassword: formData.dbPassword || undefined,
        port: formData.port || undefined,
        sshUserName: formData.sshUserName || undefined,
        sshPassword: formData.sshPassword || undefined,
        serverIP: formData.serverIP || undefined,
        dbInitialCatalog: formData.dbInitialCatalog || undefined,
        userID: formData.userID || undefined,
        password: formData.password || undefined,
        privateKeyPath: formData.privateKeyPath || undefined,
        backUpInitiatedPath: formData.backUpInitiatedPath || undefined,
        sourcepath: formData.sourcePath || undefined,
        os: formData.os || undefined,
        backUpStorageConfiguationId:
          formData.backUpStorageConfiguationId || undefined,
        backupName: description,
      });
    } else {
      return new SourceConfiguationCreateDto({
        backUPTypeId: formData.backUPType.value || "",
        dbTypeId: formData.dbType || undefined,
        databaseName: formData.databaseName || undefined,
        dbUsername: formData.dbUsername || undefined,
        dbPassword: formData.dbPassword || undefined,
        port: formData.port || undefined,
        sshUserName: formData.sshUserName || undefined,
        sshPassword: formData.sshPassword || undefined,
        serverIP: formData.serverIP || undefined,
        dbInitialCatalog: formData.dbInitialCatalog || undefined,
        userID: formData.userID || undefined,
        password: formData.password || undefined,
        privateKeyPath: formData.privateKeyPath || undefined,
        backUpInitiatedPath: formData.backUpInitiatedPath || undefined,
        sourcepath: formData.sourcePath || undefined,
        os: formData.os || undefined,
        backUpStorageConfiguationId:
          formData.backUpStorageConfiguationId || undefined,
        backupName: description,
      });
    }
  }

  deleteConfig(id: string): void {
    Swal.fire({
      title: "Are you sure?",
      text: "This will permanently delete the configuration!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#d33",
      cancelButtonColor: "#3085d6",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        this.sourceConfigs = this.sourceConfigs.filter((c) => c.id !== id);
        Swal.fire("Deleted!", "Configuration has been deleted.", "success");
      }
    });
  }

  closeDialog(): void {
    this.displayDialog = false;
  }
}