import { Component, Injector, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  Validators,
} from "@node_modules/@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import {
  BackUpStorageConfiguationCreateDto,
  BackUpStorageConfiguationServiceProxy,
  CloudStorageDto,
  CloudStorageServiceProxy,
  StorageMasterTypeDto,
  StorageMasterTypeServiceProxy,
} from "@shared/service-proxies/service-proxies";
import Swal from "sweetalert2";

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
    private backUpStorageConfiguationService: BackUpStorageConfiguationServiceProxy
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
      name: "Database Backup",
      icon: "pi pi-database",
      color: "#FFC107",
      description: "Ensure your database is safely stored and recoverable.",
      type: "DATABASE",
    },

  ];

  storageForm!: FormGroup;
  displayModal = false;
  isCloudStorage = false;
  isNFSStorage = false;
  isAWS = false;
  isAzure = false;
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
      backupName: ['', Validators.required],
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
    });
  }
  loadDestinationConfiguration(): void {
    this.backUpStorageConfiguationService.getAll(
      undefined,
      undefined,
      1000,
      0
    ).subscribe({
      next: (result) => {
         ;
        if (result && result.items) {
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

  onStorageTypeChange(event: any) {
     ;
    const selectedType = event.value?.name;
    this.resetConditionalValidators();

    if (selectedType === "Public Cloud") {
      this.isCloudStorage = true;
      this.isNFSStorage = false;
    } else if (selectedType === "Network File System") {
      this.isNFSStorage = true;
      this.isCloudStorage = false;
      this.setNfsValidators();
      this.storageForm.get("CloudStorageId")?.clearValidators();
      this.storageForm.get("CloudStorageId")?.updateValueAndValidity();
    } else {
      this.isCloudStorage = false;
      this.isNFSStorage = false;
    }
  }

  onCloudStorageChange(event: any) {
     ;
    const selectedCloud = event.value?.name;
    this.resetConditionalValidators();
    if (selectedCloud === "Amazon S3") {
      this.isAWS = true;
      this.setAwsValidators();
    } else if (selectedCloud === "Microsoft Azure") {
      this.isAzure = true;
      this.setAzureValidators();
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
    ].forEach((field) => {
      this.storageForm.get(field)?.clearValidators();
      this.storageForm.get(field)?.updateValueAndValidity();
    });

    this.isAWS = false;
    this.isAzure = false;
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

  setAzureValidators() {
    this.storageForm.get("AZ_AccountName")?.setValidators(Validators.required);
    this.storageForm.get("AZ_AccountKey")?.setValidators(Validators.required);

    this.updateValidationState();
  }

  updateValidationState() {
    Object.keys(this.storageForm.controls).forEach((field) => {
      this.storageForm.get(field)?.updateValueAndValidity();
    });
  }

  mapFormToDto(): BackUpStorageConfiguationCreateDto {
     ;
    const formValues = this.storageForm.value;
    const dto = new BackUpStorageConfiguationCreateDto();
    dto.storageMasterTypeId = formValues.StorageTypeId.value;
    dto.cloudStorageId = this.isCloudStorage
      ? formValues.CloudStorageId.value
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
    dto.awS_AccessKey = this.isAWS ? formValues.AWS_AccessKey : undefined;
    dto.awS_SecretKey = this.isAWS ? formValues.AWS_SecretKey : undefined;
    dto.awS_BucketName = this.isAWS ? formValues.AWS_BucketName : undefined;
    dto.awS_Region = this.isAWS ? formValues.AWS_Region : undefined;
    dto.awS_backUpPath = this.isAWS ? formValues.AWS_backUpPath : undefined;
    dto.aZ_AccountName = this.isAzure ? formValues.AZ_AccountName : undefined;
    dto.aZ_AccountKey = this.isAzure ? formValues.AZ_AccountKey : undefined;

    return dto;
  }

  saveStorageDestination() {
     ;
    if (this.storageForm.valid) {
      this.isSaving = true;

      const backupDto = this.mapFormToDto();
      this.backUpStorageConfiguationService.create(backupDto).subscribe({
        next: (result) => {
          console.log("Backup Configuration Saved Successfully:", result);
          this.closeDialog();
          this.isSaving = false; // Stop loader

          // Swal.fire({
          //   title: "Success!",
          //   text: "Backup Configuration has been saved successfully.",
          //   icon: "success",
          //   confirmButtonText: "OK",
          // }).then(() => {
          //   this.closeDialog(); // Close the dialog after confirmation
          // });
        },
        error: (err) => {
          console.error("Error saving backup configuration:", err);
          // Swal.fire({
          //   title: "Error!",
          //   text: "Failed to save backup configuration. Please try again.",
          //   icon: "error",
          //   confirmButtonText: "OK",
          // });
        },
      });
    } else {
      console.log("Form is invalid. Please fill all required fields.");
      // Swal.fire({
      //   title: "Warning!",
      //   text: "Form is invalid. Please fill all required fields.",
      //   icon: "warning",
      //   confirmButtonText: "OK",

      // });
      this.storageForm.markAllAsTouched();
    }
  }
}
