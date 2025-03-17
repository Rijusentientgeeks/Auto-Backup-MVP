import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
@Component({
  selector: 'app-manage-storage',
  standalone: true,
  imports: [CommonModule,CardModule],
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
  
}
