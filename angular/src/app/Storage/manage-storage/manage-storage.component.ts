import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { CardModule } from "primeng/card";
import { DropdownModule } from "primeng/dropdown";
import { InputTextModule } from "primeng/inputtext";
import { ButtonModule } from "primeng/button";
import { PasswordModule } from "primeng/password";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@node_modules/@angular/forms";
import { DialogModule } from "primeng/dialog";

@Component({
  selector: "app-manage-storage",
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    ButtonModule,
    PasswordModule,
    DialogModule,
  ],
  templateUrl: "./manage-storage.component.html",
  styleUrl: "./manage-storage.component.css",
})
export class ManageStorageComponent {
  storages = [
    {
      name: "Local Storage",
      icon: "pi pi-hdd",
      color: "#FF5722",
      description: "Store backups on your local machine or server.",
    },
    {
      name: "Network Storage",
      icon: "pi pi-globe",
      color: "#9C27B0",
      description: "Save data over the network with high availability.",
    },
    {
      name: "Cloud Storage",
      icon: "pi pi-cloud",
      color: "#03A9F4",
      description:
        "Utilize cloud platforms for remote storage and accessibility.",
    },
    {
      name: "External Drive",
      icon: "pi pi-usb",
      color: "#4CAF50",
      description: "Back up data to USB, HDD, or SSD external drives.",
    },
    {
      name: "Database Backup",
      icon: "pi pi-database",
      color: "#FFC107",
      description: "Ensure your database is safely stored and recoverable.",
    },
    {
      name: "Remote Server",
      icon: "pi pi-server",
      color: "#607D8B",
      description:
        "Backup your application data on a remote server for security.",
    },
  ];

  storageForm!: FormGroup;
  displayModal = false;
  isCloudStorage = false;
  isAWS = false;
  isAzure = false;
  openDialog() {
    this.displayModal = true;
    this.storageForm.reset();
  }
  closeDialog() {
    this.displayModal = false;
    this.storageForm.reset();

    this.resetConditionalValidators();
    this.isCloudStorage = false;
    this.isAWS = false;
    this.isAzure = false;
    this.storageForm.get("StorageTypeId")?.setValidators(Validators.required);
    this.storageForm.get("CloudStorageId")?.setValidators(Validators.required);
    this.updateValidationState();
  }
  storageTypes = [
    { name: "NFS", value: "NFS" },
    { name: "Cloud Storage", value: "CLOUD" },
  ];

  cloudStorages = [
    { name: "AWS", value: "AWS" },
    { name: "Azure", value: "AZURE" },
  ];

  constructor(private fb: FormBuilder) {
    this.initForm();
  }

  initForm() {
    this.storageForm = this.fb.group({
      StorageTypeId: [null, Validators.required],
      CloudStorageId: [null, Validators.required],
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

  saveStorage() {
    debugger;
    if (this.storageForm.valid) {
      console.log("Form Submitted", this.storageForm.value);
      this.closeDialog();
    } else {
      this.storageForm.markAllAsTouched();
    }
  }
  onStorageTypeChange(event: any) {
    const selectedType = event.value;
    this.resetConditionalValidators();

    if (selectedType === "CLOUD") {
      this.isCloudStorage = true;
    } else {
      this.isCloudStorage = false;
      this.setNfsValidators();
    }
  }

  onCloudStorageChange(event: any) {
    const selectedCloud = event.value;
    this.resetConditionalValidators();
    if (selectedCloud === "AWS") {
      this.isAWS = true;
      this.setAwsValidators();
    } else if (selectedCloud === "AZURE") {
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
    this.storageForm.get("NFS_AccessUserID")?.setValidators(Validators.required);
    this.storageForm.get("NFS_Password")?.setValidators(Validators.required);
    this.storageForm.get("NFS_LocationPath")?.setValidators(Validators.required);

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
}
