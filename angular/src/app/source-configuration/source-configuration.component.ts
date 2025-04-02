import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import {
  BackUpStorageConfiguationServiceProxy,
  BackUPTypeDto,
  BackUPTypeServiceProxy,
  DBTypeDto,
  DBTypeServiceProxy,
  SourceConfiguationCreateDto,
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
  sourceConfigs = [];
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
  constructor(
    private fb: FormBuilder,
    private BackUPTypeService: BackUPTypeServiceProxy,
    private DatabaseTypeService: DBTypeServiceProxy,
    private BackupStorageConfigService: BackUpStorageConfiguationServiceProxy,
    private sourceConfigService: SourceConfiguationServiceProxy,
    private cdr: ChangeDetectorRef
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
      userID: [""],
      password: [""],
      os: [""],
      privateKeyPath: [null],
      sourcePath: [""],
      backUpInitiatedPath: [""],
      backUpStorageConfiguationId: ["", Validators.required],
    });
  }
  onFileSelect(event: any): void {
    debugger;
    const file = event.files[0];
    if (file) {
      this.convertToBase64(file);
    }
  }
  triggerOnDemandBackup() {}
  isInvalid(controlName: string): boolean {
    const control = this.sourceForm.get(controlName);
    return control?.invalid && (control?.dirty || control?.touched);
  }

  // Convert PEM file to Base64 string
  convertToBase64(file: File): void {
    debugger;
    const reader = new FileReader();

    reader.onload = () => {
      const base64String = reader.result as string;

      // Extract the Base64 part without the data URL prefix
      const base64Data = base64String.split(",")[1];
      this.sourceForm.controls["privateKeyPath"].setValue(base64Data);
    };

    reader.onerror = (error) => {
      console.error("Error reading file:", error);
    };

    // Read the file as a data URL (Base64)
    reader.readAsDataURL(file);
  }

  loadSourceConfigs(): void {
    this.sourceConfigService.getAll(undefined, undefined, 1000, 0).subscribe({
      next: (result) => {
        if (result && result.items) {
          console.log("API Response:", result.items);
          this.sourceConfigs = result.items;

          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error("Error fetching Source Configurations:", err);
      },
    });
  }

  LoadBackupTypes(): void {
    this.BackUPTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          debugger;
          this.BackupTypes = result.items.map((item: BackUPTypeDto) => ({
            name: item.name,
            value: item.id,
          }));
        }
      },
      error: (err) => {
        console.error("Error fetching Details:", err);
      },
    });
  }

  LoadDatabaseTypes(): void {
    this.DatabaseTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          debugger;
          this.DbTypes = result.items.map((item: DBTypeDto) => ({
            name: item.name,
            value: item.id,
          }));
        }
      },
      error: (err) => {
        console.error("Error fetching Details:", err);
      },
    });
  }
  LoadBackupStorageConfigs(): void {
    this.BackupStorageConfigService.getAll(
      undefined,
      undefined,
      1000,
      0
    ).subscribe({
      next: (result) => {
        if (result && result.items) {
          this.BackupStorageConfigs = result.items.map((item: any) => ({
            name: item.cloudStorage
              ? item.cloudStorage.name
              : item.storageMasterType.name,
            value: item.id,
          }));
        }
      },
      error: (err) => {
        console.error("Error fetching Backup Storage Configurations:", err);
      },
    });
  }
  onBackupTypeChange(event: any) {
    this.showDatabaseFields = event.value?.name === "DataBase";

    if (this.showDatabaseFields) {
      this.sourceForm.controls["dbType"].setValidators(Validators.required);
      this.sourceForm.controls["serverIP"].setValidators(Validators.required);
      this.sourceForm.controls["dbInitialCatalog"].setValidators(
        Validators.required
      );
      this.sourceForm.controls["userID"].setValidators(Validators.required);
      this.sourceForm.controls["password"].setValidators(Validators.required);
      this.sourceForm.controls["os"].clearValidators();
      this.sourceForm.controls["sourcePath"].clearValidators();
    } else {
      this.sourceForm.controls["dbType"].clearValidators();
      this.sourceForm.controls["serverIP"].clearValidators();
      this.sourceForm.controls["dbInitialCatalog"].clearValidators();
      this.sourceForm.controls["userID"].clearValidators();
      this.sourceForm.controls["password"].clearValidators();
      this.sourceForm.controls["os"].setValidators(Validators.required);
      this.sourceForm.controls["sourcePath"].setValidators(Validators.required);
    }

    this.sourceForm.controls["dbType"].updateValueAndValidity();
    this.sourceForm.controls["serverIP"].updateValueAndValidity();
    this.sourceForm.controls["dbInitialCatalog"].updateValueAndValidity();
    this.sourceForm.controls["userID"].updateValueAndValidity();
    this.sourceForm.controls["password"].updateValueAndValidity();
    this.sourceForm.controls["os"].updateValueAndValidity();
    this.sourceForm.controls["sourcePath"].updateValueAndValidity();
  }
  openAddSourceDialog(): void {
    this.isEdit = false;
    this.selectedConfigId = null;
    this.sourceForm.reset();
    this.displayDialog = true;
  }

  editConfig(config: any): void {
    debugger;
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
      dbType: selectedDbType,
    });
  
    if (selectedBackupType) {
      this.onBackupTypeChange({ value: selectedBackupType });
    }
  
    this.displayDialog = true;
  }
  

  saveSourceConfig(): void {
    debugger;
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
              console.error("Error updating config:", error);
            }
          );
      } else {
        debugger;
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

              console.error("Error creating config:", error);
            }
          );
      }
    } else {
      this.sourceForm.markAllAsTouched();
    }
  }

  prepareFormData(): SourceConfiguationCreateDto | SourceConfiguationUpdateDto {
    const formData = this.sourceForm.value;

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
