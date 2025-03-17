import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
// import { CardModule } from 'primeng/card';
@Component({
  selector: 'app-manage-storage',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './manage-storage.component.html',
  styleUrl: './manage-storage.component.css'
})
export class ManageStorageComponent {
  storages = [
    { name: 'File Backup', icon: 'pi pi-folder', color: '#FFC107', description: 'Backup and restore your files securely.' },
    { name: 'Database Backup', icon: 'pi pi-database', color: '#4CAF50', description: 'Ensure your database is safely stored.' },
    { name: 'Application Backup', icon: 'pi pi-cloud', color: '#03A9F4', description: 'Save and restore application states.' }
  ];
}
