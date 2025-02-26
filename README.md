# Auto Backup MVP

Auto Backup MVP is a backup management system built with **ASP.NET Boilerplate** and **Angular**, designed for automated database, file, and S3 backups.

## Features
- **Role-Based Access:**
  - **Admins:** Configure backup settings, manage multiple sites, and have full control.
  - **Users:** Schedule and perform on-demand backups.
- **Multi-Site & Multi-Backup Support:**
  - Supports database, file system, and S3 backups.
  - Configure backup settings for multiple sites.
- **Automated & On-Demand Backups:**
  - Supports both manual and scheduled backups.
  - Logging and monitoring for backup processes.

## Installation
### Backend (ASP.NET Boilerplate)
1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/autobackup-mvp.git
   cd autobackup-mvp/backend
   ```
2. Update `appsettings.json` with your database connection.
3. Apply migrations and run the API:
   ```sh
   dotnet ef database update
   dotnet run
   ```

### Frontend (Angular)
1. Navigate to the frontend folder:
   ```sh
   cd ../frontend
   ```
2. Install dependencies and run the Angular app:
   ```sh
   npm install
   npm start
   ```

## Usage
1. Admins can log in to configure backup settings for multiple sites.
2. Users can schedule or trigger on-demand backups.
3. Backups are stored based on configuration (local, remote, or S3).

## Contributing
Pull requests are welcome! Open an issue for any feature requests or bugs.

## License
MIT License
