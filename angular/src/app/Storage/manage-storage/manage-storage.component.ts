import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@node_modules/@angular/forms';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-manage-storage',
  standalone: true,
  imports: [CommonModule,CardModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    ButtonModule,
    PasswordModule,
    DialogModule
  ],
  templateUrl: './manage-storage.component.html',
  styleUrl: './manage-storage.component.css'
})
export class ManageStorageComponent {
  storages = [
    { name: 'Local Storage', icon: 'pi pi-hdd', color: '#FF5722', description: 'Store backups on your local machine or server.' },
    { name: 'Network Storage', icon: 'pi pi-globe', color: '#9C27B0', description: 'Save data over the network with high availability.' },
    { name: 'Cloud Storage', icon: 'pi pi-cloud', color: '#03A9F4', description: 'Utilize cloud platforms for remote storage and accessibility.' },
    { name: 'External Drive', icon: 'pi pi-usb', color: '#4CAF50', description: 'Back up data to USB, HDD, or SSD external drives.' },
    { name: 'Database Backup', icon: 'pi pi-database', color: '#FFC107', description: 'Ensure your database is safely stored and recoverable.' },
    { name: 'Remote Server', icon: 'pi pi-server', color: '#607D8B', description: 'Backup your application data on a remote server for security.' }
  ];

  storageForm!: FormGroup;
  displayModal = false; // Controls modal visibility
  openDialog() {
    this.displayModal = true;
    this.storageForm.reset(); // Reset form when opening
  }
  closeDialog() {
    this.displayModal = false;
  }
  storageTypes = [
    { name: 'NFS', value: 'NFS' },
    { name: 'Cloud Storage', value: 'CLOUD' }
  ];

  cloudStorages = [
    { name: 'AWS', value: 'AWS' },
    { name: 'Azure', value: 'AZURE' }
  ];

  constructor(private fb: FormBuilder) {
    this.storageForm = this.fb.group({
      StorageTypeId: ['', Validators.required],
      CloudStorageId: ['', Validators.required],
      NFS_IP: ['', Validators.required],
      NFS_AccessUserID: ['', Validators.required],
      NFS_Password: ['', Validators.required],
      NFS_LocationPath: ['', Validators.required],
      AWS_AccessKey: ['', Validators.required],
      AWS_SecretKey: ['', Validators.required],
      AWS_BucketName: ['', Validators.required],
      AWS_Region: ['', Validators.required],
      AWS_backUpPath: ['', Validators.required],
      AZ_AccountName: ['', Validators.required],
      AZ_AccountKey: ['', Validators.required],
    });
  }


  saveStorage() {
    if (this.storageForm.valid) {
      console.log('Form Submitted', this.storageForm.value);
      this.closeDialog();
    } else {
      this.storageForm.markAllAsTouched(); // Show validation errors
    }
  }
  
}
