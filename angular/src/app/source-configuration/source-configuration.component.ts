import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
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
import Swal from "sweetalert2";

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
  osOptions = [
    { label: 'Windows', value: 'Windows' },
    { label: 'Linux', value: 'Linux' },
    { label: 'MacOS', value: 'MacOS' },
  ];
  
  constructor(
    private fb: FormBuilder,
    private BackUPTypeService: BackUPTypeServiceProxy,
    private DatabaseTypeService: DBTypeServiceProxy,
    private BackupStorageConfigService: BackUpStorageConfiguationServiceProxy,
    private sourceConfigService: SourceConfiguationServiceProxy,
    private autoBackupService: AutoBackupServiceProxy,
    private cdr: ChangeDetectorRef
  ) { }

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
      sourcePath: [''],
      backUpInitiatedPath: [''],
      backUpStorageConfiguationId: ['', Validators.required],
      backupName: [''],
      databaseName: [''],
      dbUsername: [''],
      dbPassword: [''],
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
     
    this.autoBackupService.createBackup(id).subscribe({
      next: (result) => {
        if (result) {
          Swal.fire("Success", "Backup Created Successfully", "success");
        }
      }
    });
  }
  isInvalid(controlName: string): boolean {
    const control = this.sourceForm.get(controlName);
    return control?.invalid && (control?.dirty || control?.touched);
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

    reader.onerror = (error) => {
    };

    // Read the file as a data URL (Base64)
    reader.readAsDataURL(file);
  }

  loadSourceConfigs(): void {
    this.sourceConfigService.getAll(undefined, undefined, 1000, 0).subscribe({
      next: (result) => {        
        if (result && result.items) {
          this.sourceConfigs = result.items;
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
      },
    });
  }

  LoadBackupTypes(): void {
    this.BackUPTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {           
          this.BackupTypes = result.items.map(
            (item: BackUPTypeDto) => ({
              name: item.name,
              value: item.id,
            })
          );
        }
      },
      error: (err) => {
      },
    });
  }

  LoadDatabaseTypes(): void {
    this.DatabaseTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {           
          this.DbTypes = result.items.map(
            (item: DBTypeDto) => ({
              name: item.name,
              value: item.id,
            })
          );
        }
      },
      error: (err) => {
      },
    });
  }
  LoadBackupStorageConfigs(): void {
    this.BackupStorageConfigService.getAll(
      undefined,
      undefined,
      1000,
      0).subscribe({
        next: (result) => {
          if (result && result.items) {
            this.BackupStorageConfigs = result.items.map((item: any) => ({
              // name: item.cloudStorage ? item.cloudStorage.name : item.storageMasterType.name,
              name: item.backupName,
              value: item.id,
            }));
          }
        },
        error: (err) => {
        },
      });
  }
  onBackupTypeChange(event: any) {
    this.showDatabaseFields = event.value?.name === 'DataBase';
    if (this.showDatabaseFields) {
      this.sourceForm.controls["dbType"].setValidators(Validators.required);
      this.sourceForm.controls["serverIP"].setValidators(Validators.required);
      // this.sourceForm.controls["dbInitialCatalog"].setValidators(Validators.required);
      this.sourceForm.controls["sshUserName"].setValidators(Validators.required);
      this.sourceForm.controls["sshPassword"].clearValidators();
      this.sourceForm.controls["databaseName"].setValidators(Validators.required);
      this.sourceForm.controls["dbUsername"].setValidators(Validators.required);
      this.sourceForm.controls["dbPassword"].setValidators(Validators.required); 
      this.sourceForm.controls["os"].clearValidators();
      this.sourceForm.controls["sourcepath"].clearValidators();
      this.sourceForm.controls["privateKeyPath"].clearValidators();
      this.sourceForm.controls["backUpInitiatedPath"].clearValidators();
    } else {
      this.sourceForm.controls["dbType"].clearValidators();
      this.sourceForm.controls["serverIP"].clearValidators();
      // this.sourceForm.controls["dbInitialCatalog"].clearValidators();
      this.sourceForm.controls["sshUserName"].clearValidators();
      this.sourceForm.controls["sshPassword"].clearValidators();
      this.sourceForm.controls["os"].clearValidators();
      this.sourceForm.controls["privateKeyPath"].clearValidators();
      this.sourceForm.controls["backUpInitiatedPath"].clearValidators();
      this.sourceForm.controls["sourcepath"].setValidators(Validators.required);
    }

    this.sourceForm.controls['dbType'].updateValueAndValidity();
    this.sourceForm.controls['serverIP'].updateValueAndValidity();
    // this.sourceForm.controls['dbInitialCatalog'].updateValueAndValidity();
    this.sourceForm.controls['sshUserName'].updateValueAndValidity();
    this.sourceForm.controls['sshPassword'].updateValueAndValidity();
    this.sourceForm.controls['databaseName'].updateValueAndValidity();
    this.sourceForm.controls['dbUsername'].updateValueAndValidity();
    this.sourceForm.controls['dbPassword'].updateValueAndValidity();
    this.sourceForm.controls['os'].updateValueAndValidity();
    this.sourceForm.controls['sourcepath'].updateValueAndValidity();
    this.sourceForm.controls['backUpInitiatedPath'].updateValueAndValidity();
    this.sourceForm.controls['privateKeyPath'].updateValueAndValidity();
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
      backUPType: selectedBackupType,
      dbType: selectedDbType?selectedDbType.value: null,
    });
    setTimeout(() => {
      if (selectedBackupType) {
        this.onBackupTypeChange({ value: selectedBackupType });
      }
    }, 0);
  
    this.displayDialog = true;
  }
  
  private isPasswordOrPrivateKeyProvided(): boolean {
    const password = this.sourceForm.controls['sshPassword'].value;
    const privateKey = this.sourceForm.controls['privateKeyPath'].value;
    return !!password || !!privateKey;
  }
  
  saveSourceConfig(): void {
     if (!this.isPasswordOrPrivateKeyProvided()) {
      this.sourceForm.controls['sshPassword'].markAsTouched();
      Swal.fire("Error!", "Please provide either Password or Private Key File.", "error");

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
              this.loadSourceConfigs();
            },
            (error) => {
              this.isSaving = false;
            }
          );
      } else {
         
        this.sourceConfigService
          .create(formData as SourceConfiguationCreateDto)
          .subscribe(
            () => {
              this.closeDialog();
              this.isSaving = false;
              this.loadSourceConfigs();
            },
            (error) => {
              this.isSaving = false;
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
      description = `${formData.backupName || 'Backup'} - ${formData.backUPType?.name || 'Type'} - ${formData.serverIP || 'Server'}`;
    }
    if (this.isEdit && this.selectedConfigId) {
      return new SourceConfiguationUpdateDto({
        id: this.selectedConfigId,
        backUPTypeId: formData.backUPType.value || '',
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
        backUpStorageConfiguationId: formData.backUpStorageConfiguationId || undefined,
        backupName: description,
      });
    } else {
      return new SourceConfiguationCreateDto({
        backUPTypeId: formData.backUPType.value || '',
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
        backUpStorageConfiguationId: formData.backUpStorageConfiguationId || undefined,
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
