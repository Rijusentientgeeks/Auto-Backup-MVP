import { Component, Injector, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  Validators,
} from "@node_modules/@angular/forms";
import { MessageService } from "primeng/api";
import { AppComponentBase } from "@shared/app-component-base";
import {
  BackUpStorageConfiguationCreateDto,
  BackUpStorageConfiguationServiceProxy,
  BackUpStorageConfiguationUpdateDto,
  CloudStorageDto,
  CloudStorageServiceProxy,
  StorageMasterTypeDto,
  StorageMasterTypeServiceProxy,
} from "@shared/service-proxies/service-proxies";

@Component({
  selector: "app-manage-storage",
  templateUrl: "./manage-storage.component.html",
  styleUrl: "./manage-storage.component.css",
})
export class ManageStorageComponent extends AppComponentBase implements OnInit {
  constructor(
    private fb: FormBuilder,
    injector: Injector,
    private storageMasterTypeService: StorageMasterTypeServiceProxy,
    private cloudStorageService: CloudStorageServiceProxy,
    private backUpStorageConfiguationService: BackUpStorageConfiguationServiceProxy,
        private messageService: MessageService
    
  ) {
    super(injector);
    this.initForm();
  }
  storages = [
    {
      name: "Network Storage",
      icon: "pi pi-globe",
      color: "#9C27B0",
      description: "Save data over the network with high availability.",
      type: "Network File System",
    },
    {
      name: "Cloud Storage",
      icon: "pi pi-cloud",
      color: "#03A9F4",
      description:
        "Utilize cloud platforms for remote storage and accessibility.",
      type: "Public Cloud",
    },

    {
      name: "GeekSync Cluster",
      icon: "pi pi-database",
      color: "#FFC107",
      description: "Ensure your Files are safely stored and recoverable.",
      type: "DATABASE",
    },
  ];

  storageForm!: FormGroup;
  displayModal = false;
  isCloudStorage = false;
  isNFSStorage = false;
  isAWS = false;
  isAzure = false;
  isAlibaba = false;
  isGoogle = false;
  storageTypes = [];
  cloudStorages = [];
  selectedStorage: string | null = null;
  showDetail: boolean = false;
  isSaving: boolean = false;
  storageEntries = [];
  selectStorage(storageName: string) {
    this.selectedStorage = storageName;
    this.showDetail = true;
  }
  goBack() {
    this.showDetail = false;
    this.loadStorageTypes();
    this.loadCloudStorageTypes();
    this.selectedStorage = "";
    this.loadDestinationConfiguration();
  }
  openDialog() {
    this.displayModal = true;
    this.storageForm.reset();
  }
  closeDialog() {
    this.displayModal = false;
    this.storageForm.reset();
    this.resetConditionalValidators();
    this.isCloudStorage = false;
    this.isNFSStorage = false;
    this.isAWS = false;
    this.isAzure = false;
    this.storageForm.get("StorageTypeId")?.setValidators(Validators.required);
    this.storageForm.get("CloudStorageId")?.setValidators(Validators.required);
    this.updateValidationState();
  }

  ngOnInit(): void {
    this.loadStorageTypes();
    this.loadCloudStorageTypes();
    this.selectedStorage = "";
    this.loadDestinationConfiguration();
  }

  initForm() {
    this.storageForm = this.fb.group({
      StorageTypeId: [null, Validators.required],
      CloudStorageId: [null, Validators.required],
      backupName: ["", Validators.required],
      NFS_IP: [null],
      NFS_AccessUserID: [null],
      NFS_Password: [null],
      NFS_LocationPath: [null],
      AWS_AccessKey: [null],
      AWS_SecretKey: [null],
      AWS_BucketName: [null],
      AWS_Region: [null],
      AWS_backUpPath: [null],
      AZ_AccountName: [null],
      AZ_AccountKey: [null],
      Endpoint: [null],
      ProjectID: [null],
      CredentialFile: [null],
    });
  }
  loadDestinationConfiguration(): void {
    this.backUpStorageConfiguationService
      .getAll(undefined, undefined, 1000, 0)
      .subscribe({
        next: (result) => {
          if (result && result.items) {
            debugger;
            this.storageEntries = result.items;
          }
        },
        error: (err) => {
          console.error("Error fetching storage destination:", err);
        },
      });
  }

  loadStorageTypes(): void {
    this.storageMasterTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          this.storageTypes = result.items.map(
            (item: StorageMasterTypeDto) => ({
              name: item.name,
              value: item.id,
            })
          );
        }
      },
      error: (err) => {
        console.error("Error fetching storage types:", err);
      },
    });
  }
  loadCloudStorageTypes(): void {
    this.cloudStorageService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          this.cloudStorages = result.items.map((item: CloudStorageDto) => ({
            name: item.name,
            value: item.id,
          }));
        }
      },
      error: (err) => {
        console.error("Error fetching storage types:", err);
      },
    });
  }
  onEditStorage(entry: any): void {
    debugger;
    this.editingStorageId = entry.id;

    this.displayModal = true;
    this.storageForm.reset();
    this.storageForm.patchValue({
      StorageTypeId: entry.storageMasterTypeId,
      CloudStorageId: entry.cloudStorageId,
      backupName: entry.backupName,
      AWS_AccessKey: entry.awS_AccessKey,
      AWS_SecretKey: entry.awS_SecretKey,
      AWS_BucketName: entry.awS_BucketName,
      AWS_Region: entry.awS_Region,
      AWS_backUpPath: entry.awS_backUpPath,
      AZ_AccountName: entry.aZ_AccountName,
      AZ_AccountKey: entry.aZ_AccountKey,
      NFS_IP: entry.nfS_IP,
      NFS_AccessUserID: entry.nfS_AccessUserID,
      NFS_Password: entry.nfS_Password,
      NFS_LocationPath: entry.nfS_LocationPath,
      Endpoint: entry.endpoint,
      ProjectID: entry.projectID,
      CredentialFile: entry.credentialFile,

    });

    const selectedType = this.storageTypes.find(
      (x) => x.value === entry.storageMasterTypeId
    );
    if (selectedType) {
      this.onStorageTypeChange({ value: selectedType.value });
    }

    if (entry.cloudStorageId) {
      const selectedCloud = this.cloudStorages.find(
        (x) => x.value === entry.cloudStorageId
      );

      if (selectedCloud) {
        this.onCloudStorageChange({ value: selectedCloud.value });

        const name = selectedCloud.name?.toLowerCase();
        this.isCloudStorage = true;
        debugger;

        if (name === "amazon s3") {
          this.isAWS = true;
          this.setAwsValidators();
        } else if (name === "microsoft azure") {
          this.isAzure = true;
          this.setAzureValidators();
        } else if (name === "Alibaba Cloud") {
          this.isAlibaba = true;
          this.setAlibabaValidators();
        }
         else if (name === "Google Cloud") {
          this.isGoogle = true;
          this.setGCValidators();
        }
      }
    } else if (entry.storageType === "NFS") {
      this.isNFSStorage = true;
      this.setNfsValidators();
    }

    this.showDetail = false;
  }

  onStorageTypeChange(event: { value: string }) {
    debugger;
    const selectedType = this.storageTypes.find((x) => x.value === event.value);

    this.resetConditionalValidators();
    if (selectedType.name === "Public Cloud") {
      this.isCloudStorage = true;
      this.isNFSStorage = false;
      this.isAWS = false;
      this.isAzure = false;
      this.isAlibaba = false;
    } else if (selectedType.name === "Network File System") {
      this.isNFSStorage = true;
      this.isCloudStorage = false;
      this.isAWS = false;
      this.isAzure = false;
      this.isAlibaba = false;
      this.setNfsValidators();
      this.storageForm.get("CloudStorageId")?.clearValidators();
      this.storageForm.get("CloudStorageId")?.updateValueAndValidity();
    }   
     else {
      this.isCloudStorage = false;
      this.isNFSStorage = false;
      this.isAWS = false;
      this.isAzure = false;
      this.isAlibaba = false;
    }
  }

  onCloudStorageChange(event: { value: string }) {
    debugger;

    const selectedCloud = this.cloudStorages.find(
      (x) => x.value === event.value
    );
    this.resetConditionalValidators();
    if (selectedCloud.name === "Amazon S3") {
      this.isAWS = true;
      this.isAzure = false;
      this.isAlibaba = false;
      this.setAwsValidators();
    } else if (selectedCloud.name === "Microsoft Azure") {
      this.isAzure = true;
      this.isAWS = false;
      this.isAlibaba = false;
      this.setAzureValidators();
    } else if (selectedCloud.name === "Alibaba Cloud") {
      this.isAlibaba = true;
      this.isAWS = false;
      this.isAzure = false;
      this.setAlibabaValidators();
    }
    else if (selectedCloud.name === "Google Cloud") {
      this.isGoogle = true;
      this.isAWS = false;
      this.isAzure = false;
      this.isAlibaba = false;
      this.setGCValidators();
    } else {
      this.isAWS = false;
      this.isAzure = false;
      this.isAlibaba = false;
      this.isGoogle = false;
    }
  }
  resetConditionalValidators() {
    [
      "NFS_IP",
      "NFS_AccessUserID",
      "NFS_Password",
      "NFS_LocationPath",
      "AWS_AccessKey",
      "AWS_SecretKey",
      "AWS_BucketName",
      "AWS_Region",
      "AWS_backUpPath",
      "AZ_AccountName",
      "AZ_AccountKey",
      "Endpoint",
      "ProjectID",
      "CredentialFile",


    ].forEach((field) => {
      this.storageForm.get(field)?.clearValidators();
      this.storageForm.get(field)?.updateValueAndValidity();
    });

    this.isAWS = false;
    this.isAzure = false;
    this.isAlibaba = false;
    this.isGoogle = false;

  }

  isInvalid(controlName: string): boolean {
    const control = this.storageForm.get(controlName);
    return control?.invalid && control?.touched;
  }

  setNfsValidators() {
    this.storageForm.get("NFS_IP")?.setValidators(Validators.required);
    this.storageForm
      .get("NFS_AccessUserID")
      ?.setValidators(Validators.required);
    this.storageForm.get("NFS_Password")?.setValidators(Validators.required);
    this.storageForm
      .get("NFS_LocationPath")
      ?.setValidators(Validators.required);
    this.updateValidationState();
  }

  setAwsValidators() {
    this.storageForm.get("AWS_AccessKey")?.setValidators(Validators.required);
    this.storageForm.get("AWS_SecretKey")?.setValidators(Validators.required);
    this.storageForm.get("AWS_BucketName")?.setValidators(Validators.required);
    this.storageForm.get("AWS_Region")?.setValidators(Validators.required);
    this.storageForm.get("AWS_backUpPath")?.setValidators(Validators.required);
    this.updateValidationState();
  }

  setAlibabaValidators() {
    this.storageForm.get("AWS_AccessKey")?.setValidators(Validators.required);
    this.storageForm.get("AWS_SecretKey")?.setValidators(Validators.required);
    this.storageForm.get("AWS_BucketName")?.setValidators(Validators.required);
    this.storageForm.get("Endpoint")?.setValidators(Validators.required);
    this.updateValidationState();
  }
  setGCValidators() {
    this.storageForm.get("ProjectID")?.setValidators(Validators.required);
    this.storageForm.get("CredentialFile")?.setValidators(Validators.required);
    this.storageForm.get("AWS_BucketName")?.setValidators(Validators.required);
    this.updateValidationState();
  }
  setAzureValidators() {
    this.storageForm.get("AZ_AccountName")?.setValidators(Validators.required);
    this.storageForm.get("AZ_AccountKey")?.setValidators(Validators.required);
    this.storageForm.get("Endpoint")?.setValidators(Validators.required);
    this.updateValidationState();
  }

  updateValidationState() {
    Object.keys(this.storageForm.controls).forEach((field) => {
      this.storageForm.get(field)?.updateValueAndValidity();
    });
  }

  mapFormToCreateDto(): BackUpStorageConfiguationCreateDto {
    const formValues = this.storageForm.value;
    const dto = new BackUpStorageConfiguationCreateDto();
    dto.storageMasterTypeId = formValues.StorageTypeId;
    dto.cloudStorageId = this.isCloudStorage
      ? formValues.CloudStorageId
      : undefined;
    dto.backupName = formValues.backupName;
    dto.nfS_IP = !this.isCloudStorage ? formValues.NFS_IP : undefined;
    dto.nfS_AccessUserID = !this.isCloudStorage
      ? formValues.NFS_AccessUserID
      : undefined;
    dto.nfS_Password = !this.isCloudStorage
      ? formValues.NFS_Password
      : undefined;
    dto.nfS_LocationPath = !this.isCloudStorage
      ? formValues.NFS_LocationPath
      : undefined;
    dto.awS_AccessKey = this.isAWS || this.isAlibaba ? formValues.AWS_AccessKey : undefined;
    dto.awS_SecretKey = this.isAWS || this.isAlibaba? formValues.AWS_SecretKey : undefined;
    dto.awS_BucketName = this.isAWS || this.isAlibaba || this.isGoogle? formValues.AWS_BucketName : undefined;
    dto.awS_Region = this.isAWS ? formValues.AWS_Region : undefined;
    dto.awS_backUpPath = this.isAWS ? formValues.AWS_backUpPath : undefined;
    dto.aZ_AccountName = this.isAzure ? formValues.AZ_AccountName : undefined;
    dto.aZ_AccountKey = this.isAzure ? formValues.AZ_AccountKey : undefined;
    dto.endpoint = this.isAlibaba || this.isAzure ? formValues.Endpoint : undefined;
    dto.projectID = this.isGoogle ? formValues.ProjectID : undefined;
    debugger
    dto.credentialFile = this.isGoogle ? formValues.CredentialFile : undefined;
    return dto;
  }

  mapFormToUpdateDto(): BackUpStorageConfiguationUpdateDto {
    const formValues = this.storageForm.value;
    const dto = new BackUpStorageConfiguationUpdateDto();

    dto.id = this.editingStorageId!;
    dto.storageMasterTypeId = formValues.StorageTypeId;
    dto.cloudStorageId = this.isCloudStorage
      ? formValues.CloudStorageId
      : undefined;
    dto.backupName = formValues.backupName;
    dto.nfS_IP = !this.isCloudStorage ? formValues.NFS_IP : undefined;
    dto.nfS_AccessUserID = !this.isCloudStorage
      ? formValues.NFS_AccessUserID
      : undefined;
    dto.nfS_Password = !this.isCloudStorage
      ? formValues.NFS_Password
      : undefined;
    dto.nfS_LocationPath = !this.isCloudStorage
      ? formValues.NFS_LocationPath
      : undefined;
    dto.awS_AccessKey = this.isAWS || this.isAlibaba? formValues.AWS_AccessKey : undefined;
    dto.awS_SecretKey = this.isAWS|| this.isAlibaba ? formValues.AWS_SecretKey : undefined;
    dto.awS_BucketName = this.isAWS|| this.isAlibaba || this.isGoogle ? formValues.AWS_BucketName : undefined;
    dto.awS_Region = this.isAWS ? formValues.AWS_Region : undefined;
    dto.awS_backUpPath = this.isAWS ? formValues.AWS_backUpPath : undefined;
    dto.aZ_AccountName = this.isAzure ? formValues.AZ_AccountName : undefined;
    dto.aZ_AccountKey = this.isAzure ? formValues.AZ_AccountKey : undefined;
    dto.projectID = this.isGoogle ? formValues.ProjectID : undefined;
    dto.credentialFile = this.isGoogle ? formValues.CredentialFile : undefined;
    dto.endpoint = this.isAlibaba || this.isAzure ? formValues.Endpoint : undefined;
    return dto;
  }
  editingStorageId: string | undefined;

  saveStorageDestination() {
    if (this.storageForm.valid) {
      this.isSaving = true;

      const dto = this.editingStorageId
        ? this.mapFormToUpdateDto()
        : this.mapFormToCreateDto();

      let request$;
      if (this.editingStorageId) {
        const updateDto = this.mapFormToUpdateDto();
        request$ = this.backUpStorageConfiguationService.update(updateDto);
      } else {
        const createDto = this.mapFormToCreateDto();
        request$ = this.backUpStorageConfiguationService.create(createDto);
      }

      request$.subscribe({
        next: (result) => {
          this.messageService.add({
            severity: "success",
            summary: "success",
            detail: "Backup Configuration Saved successfully!",
            life: 2000,
          });
          this.closeDialog();
          this.isSaving = false;
          this.loadStorageTypes();
          this.loadCloudStorageTypes();
          this.selectedStorage = "";
          this.loadDestinationConfiguration();
        },
        error: (err) => {
          console.error("Error saving backup configuration:", err);
          this.messageService.add({
            severity: "error",
            summary: "Error",
            detail: "Failed to save Backup Configuration!",
            life: 2000,
          });
          this.isSaving = false;
        },
      });
    } else {
      this.storageForm.markAllAsTouched();
      this.messageService.add({
        severity: "error",
        summary: "Error",
        detail: "Please fill all required fields.",
        life: 2000,
      });
      this.isSaving = false;
    }
  }
}
