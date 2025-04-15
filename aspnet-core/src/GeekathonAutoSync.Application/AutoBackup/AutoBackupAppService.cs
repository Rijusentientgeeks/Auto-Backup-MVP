using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CronExpressionDescriptor;
using GeekathonAutoSync.AutoBackup.Dto;
using GeekathonAutoSync.BackUpLogs;
using GeekathonAutoSync.BackUpSchedules;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.CloudStorages;
using GeekathonAutoSync.DBTypes;
using GeekathonAutoSync.SourceConfiguations;
using GeekathonAutoSync.StorageMasterTypes;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup
{
    public class AutoBackupAppService : GeekathonAutoSyncAppServiceBase, IAutoBackupAppService
    {
        private readonly IRepository<BackUpStorageConfiguation, Guid> _backUpStorageConfiguationRepository;
        private readonly IRepository<SourceConfiguation, Guid> _sourceConfiguationRepository;
        private readonly IRepository<BackUpLog, Guid> _backUpLogsRepository; 
        private readonly IWebHostEnvironment _env;
        private readonly IRepository<BackUpSchedule, Guid> _backUpScheduleRepository;
        private string defaultInitialLocalPath = "LocalBackupFiles";

        public AutoBackupAppService(IRepository<BackUpStorageConfiguation, Guid> backUpStorageConfiguationRepository,
            IRepository<StorageMasterType, Guid> storageMasterTypeRepository,
            IRepository<CloudStorage, Guid> cloudStorageRepository, IRepository<SourceConfiguation, Guid> sourceConfiguationRepository, IWebHostEnvironment env,
            IRepository<BackUpLog, Guid> backUpLogsRepository,
            IRepository<BackUpSchedule, Guid> backUpScheduleRepository)
        {
            _backUpStorageConfiguationRepository = backUpStorageConfiguationRepository;
            _sourceConfiguationRepository = sourceConfiguationRepository;
            _env = env;
            _backUpLogsRepository = backUpLogsRepository;
            _backUpScheduleRepository = backUpScheduleRepository;
        }
        public async Task<string> CreateBackup(string sConfigurationId)
        {
            int tenantId = GetCurrentTenantId();
            var backupFileName = "";
            string downLoadedFileName = "";
            bool fileDownloadflag = false;
            string resFromUpload = "";

            string serverPath = Path.Combine(_env.ContentRootPath, defaultInitialLocalPath);

            var BackUPConfig = await _sourceConfiguationRepository.GetAll()
                .Include(i => i.BackUPType)
                .Include(i => i.DBType)
                .Include(i => i.BackUpStorageConfiguation).ThenInclude(e => e.StorageMasterType)
                .Include(i => i.BackUpStorageConfiguation).ThenInclude(e => e.CloudStorage)
                .FirstOrDefaultAsync(i => i.Id.ToString().ToLower() == sConfigurationId.ToLower());

            //insert to BackLog
            var backupLogResult = await CreatebackLog(BackUPConfig.Id, DateTime.UtcNow, (Guid)BackUPConfig.BackUpStorageConfiguationId, tenantId);
            try
            {
                (bool, string) resp = (false, "");
                switch (BackUPConfig.BackUPType.BackupTypeEnum)
                {
                    case BackupTypeEnum.DataBase:
                        switch (BackUPConfig.DBType.Type)
                        {
                            case DbTypeEnum.PostgreSQL:
                                resp = await PostGresDBBackUp(BackUPConfig.BackUpInitiatedPath, BackUPConfig.SshUserName, BackUPConfig.SshPassword, BackUPConfig.ServerIP, BackUPConfig.DbPassword, BackUPConfig.DbUsername, BackUPConfig.DatabaseName);
                                break;
                            case DbTypeEnum.MicrosoftSQLServer:
                                resp = await MSSQLBackUp(BackUPConfig.ServerIP, BackUPConfig.DBInitialCatalog, BackUPConfig.DbUsername, BackUPConfig.DbPassword, BackUPConfig.BackUpInitiatedPath, BackUPConfig.DatabaseName);
                                break;
                            case DbTypeEnum.OracleDatabase:
                                resp = (false, "Work Inprogress.");
                                break;
                            case DbTypeEnum.MySQL:
                                resp = (false, "Work Inprogress.");
                                break;
                            case DbTypeEnum.MongoDB:
                                resp = (false, "Work Inprogress.");
                                break;
                            default:
                                resp = (false, BackUPConfig.DBType.Name + " database type is not valid.");
                                break;
                                //backupFileName = res;
                        }
                        backupFileName = resp.Item2;
                        string nameWithoutExtension = Path.GetFileNameWithoutExtension(backupFileName);
                        string backupZipFileWithPath = Path.Combine(BackUPConfig.BackUpInitiatedPath, $"{nameWithoutExtension}.zip");

                        downLoadedFileName = $"{nameWithoutExtension}.zip";
                        var response = await ZipAndDownloadBackupSSHToLocal(BackUPConfig.ServerIP, BackUPConfig.SshUserName, BackUPConfig.SshPassword, BackUPConfig.BackUpInitiatedPath, backupZipFileWithPath, Path.Combine(serverPath, downLoadedFileName), backupFileName, BackUPConfig.PrivateKeyPath);
                        fileDownloadflag = resp.Item1;
                        break;

                    case BackupTypeEnum.ApplicationFiles:

                        resp = await ApplicationBackup(BackUPConfig.Sourcepath, BackUPConfig.BackUpInitiatedPath, BackUPConfig.PrivateKeyPath, BackUPConfig.SshUserName, BackUPConfig.ServerIP, BackUPConfig.SshPassword, serverPath);
                        downLoadedFileName = resp.Item2;
                        fileDownloadflag = resp.Item1;
                        break;

                    case BackupTypeEnum.SpecificFile:
                        break;
                    default:
                        break;
                }


                if (fileDownloadflag == true)
                {
                    var storageType = BackUPConfig.BackUpStorageConfiguation.StorageMasterType.Type;
                    var cloudStorageType = BackUPConfig.BackUpStorageConfiguation?.CloudStorage.Type;

                    switch (storageType)
                    {
                        case StorageMasterTypeEnum.PublicCloud:
                            switch (cloudStorageType)
                            {
                                case CloudStorageType.AmazonS3:
                                    string downloadedFileWithPath = Path.Combine(serverPath, downLoadedFileName);
                                    resFromUpload = await UploadFileToS3(BackUPConfig.BackUpStorageConfiguation.AWS_AccessKey, BackUPConfig.BackUpStorageConfiguation.AWS_SecretKey, BackUPConfig.BackUpStorageConfiguation.AWS_BucketName, $"{BackUPConfig.BackUpStorageConfiguation.AWS_backUpPath.TrimEnd('/')}/{downLoadedFileName}", $"{downloadedFileWithPath}");
                                    break;
                                case CloudStorageType.MicrosoftAzure:
                                    break;
                                case CloudStorageType.GoogleCloud:
                                    break;
                                case CloudStorageType.AlibabaCloud:
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case StorageMasterTypeEnum.NFS:
                            break;
                        case StorageMasterTypeEnum.GeekSyncInfrastructureCluste:
                            break;
                        default:
                            break;

                    }
                }
                await UpdatebackLogStatus(backupLogResult.Item2, DateTime.UtcNow, BackupLogStatus.Success, downLoadedFileName, "");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }


            return resFromUpload;
        }


        public async Task<string> CreateBackupAndDownload(string sConfigurationId)
        {
            int tenantId = GetCurrentTenantId();
            var backupFileName = "";
            string downLoadedFileName = "";
            bool fileDownloadflag = false;
            string resFromUpload = "";
            string localhostPath = string.Empty;

            string serverPath = Path.Combine(_env.ContentRootPath, defaultInitialLocalPath);

            try
            {
            var BackUPConfig = await _sourceConfiguationRepository.GetAll()
                .Include(i => i.BackUPType)
                .Include(i => i.DBType)
                .Include(i => i.BackUpStorageConfiguation).ThenInclude(e => e.StorageMasterType)
                .Include(i => i.BackUpStorageConfiguation).ThenInclude(e => e.CloudStorage)
                .FirstOrDefaultAsync(i => i.Id.ToString().ToLower() == sConfigurationId.ToLower());

            //insert to BackLog
            var backupLogResult = await CreatebackLog(BackUPConfig.Id, DateTime.UtcNow, (Guid)BackUPConfig.BackUpStorageConfiguationId, tenantId);
                (bool, string) resp = (false, "");
                switch (BackUPConfig.BackUPType.BackupTypeEnum)
                {
                    case BackupTypeEnum.DataBase:
                        switch (BackUPConfig.DBType.Type)
                        {
                            case DbTypeEnum.PostgreSQL:
                                resp = await PostGresDBBackUp(BackUPConfig.BackUpInitiatedPath, BackUPConfig.SshUserName, BackUPConfig.SshPassword, BackUPConfig.ServerIP, BackUPConfig.DbPassword, BackUPConfig.DbUsername, BackUPConfig.DatabaseName);
                                break;
                            case DbTypeEnum.MicrosoftSQLServer:
                                resp = await MSSQLBackUp(BackUPConfig.ServerIP, BackUPConfig.DBInitialCatalog, BackUPConfig.DbUsername, BackUPConfig.DbPassword, BackUPConfig.BackUpInitiatedPath, BackUPConfig.DatabaseName);
                                break;
                            case DbTypeEnum.OracleDatabase:
                                resp = (false, "Work Inprogress.");
                                break;
                            case DbTypeEnum.MySQL:
                                resp = (false, "Work Inprogress.");
                                break;
                            case DbTypeEnum.MongoDB:
                                resp = (false, "Work Inprogress.");
                                break;
                            default:
                                resp = (false, BackUPConfig.DBType.Name + " database type is not valid.");
                                break;
                                //backupFileName = res;
                        }
                        backupFileName = resp.Item2;
                        string nameWithoutExtension = Path.GetFileNameWithoutExtension(backupFileName);
                        string backupZipFileWithPath = Path.Combine(BackUPConfig.BackUpInitiatedPath, $"{nameWithoutExtension}.zip");

                        downLoadedFileName = $"{nameWithoutExtension}.zip";
                        var response = await ZipAndDownloadBackupSSHToLocal(BackUPConfig.ServerIP, BackUPConfig.SshUserName, BackUPConfig.SshPassword, BackUPConfig.BackUpInitiatedPath, backupZipFileWithPath, Path.Combine(serverPath, downLoadedFileName), backupFileName, BackUPConfig.PrivateKeyPath);
                        fileDownloadflag = resp.Item1;
                        break;

                    case BackupTypeEnum.ApplicationFiles:

                        resp = await ApplicationBackup(BackUPConfig.Sourcepath, BackUPConfig.BackUpInitiatedPath, BackUPConfig.PrivateKeyPath, BackUPConfig.SshUserName, BackUPConfig.ServerIP, BackUPConfig.SshPassword, serverPath);
                        downLoadedFileName = resp.Item2;
                        fileDownloadflag = resp.Item1;
                        break;

                    case BackupTypeEnum.SpecificFile:
                        break;
                    default:
                        break;
                }

                localhostPath = Path.Combine(serverPath, downLoadedFileName);
                
                await UpdatebackLogStatus(backupLogResult.Item2, DateTime.UtcNow, BackupLogStatus.LocalBackup, string.Empty, "");
            }
            catch (Exception ex)
            {
                //throw new UserFriendlyException(ex.Message);
            }


            return localhostPath;
        }

        public async Task<Stream> GetBackupFromLocalHost(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            var dir = Path.GetDirectoryName(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Backup file not found.", filePath);
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            
            memory.Position = 0;
            return memory;
        }


        private int GetCurrentTenantId()
        {
            if (AbpSession.TenantId.HasValue)
            {
                return AbpSession.TenantId.Value;
            }

            if (UnitOfWorkManager?.Current?.GetTenantId() != null)
            {
                return UnitOfWorkManager.Current.GetTenantId().Value;
            }

            throw new UserFriendlyException("TenantId is not available in the current context.");
        }

        public async Task<PagedResultDto<BackUpLogDto>> GetAllBackupLogAsync(PagedBackLogRequestDto input)
        {
            try
            {
                var query = _backUpLogsRepository.GetAll()
                    .Include(i => i.SourceConfiguation)
                    .Include(i => i.BackUpStorageConfiguation)
                    .Where(i => i.TenantId == AbpSession.TenantId);
                var c = query.ToList();

                var filteredQuery = query
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                        u => !string.IsNullOrEmpty(u.SourceConfiguation?.BackupName) && u.SourceConfiguation.BackupName.ToLower().Contains(input.Keyword.ToLower()) ||
                             !string.IsNullOrEmpty(u.BackUpFileName) && u.BackUpFileName.ToLower().Contains(input.Keyword.ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(input.SourceConfigId),
                        u => u.SourceConfiguationId.ToString().ToLower() == input.SourceConfigId.ToLower())
                    .WhereIf(!string.IsNullOrEmpty(input.BackupStorageid),
                        u => u.BackUpStorageConfiguation.StorageMasterTypeId.ToString().ToLower() == input.BackupStorageid.ToLower())
                    .WhereIf(!string.IsNullOrEmpty(input.BackupTypeId),
                        u => u.SourceConfiguation.BackUPTypeId.ToString().ToLower() == input.BackupTypeId.ToLower())
                    .WhereIf(!string.IsNullOrEmpty(input.CloudStorageTypeId),
                        u => u.BackUpStorageConfiguation.CloudStorageId.ToString().ToLower() == input.CloudStorageTypeId.ToLower());

                var totalCount = filteredQuery.Count();

                var pagedItems = filteredQuery
                    .OrderByDescending(x => x.StartedTimeStamp) 
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();

                return new PagedResultDto<BackUpLogDto>
                {
                    TotalCount = totalCount,
                    Items = ObjectMapper.Map<List<BackUpLogDto>>(pagedItems)
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<BackUpLogDto>> GetAllCompletedBackupLogByStorageConfigIdAsync(PagedBackupLogInputDto input)
            {
            try
            {
                var query = _backUpLogsRepository.GetAll()
                .Include(i => i.BackUpStorageConfiguation)
                .Where(
                    i => i.TenantId == AbpSession.TenantId
                    && i.BackUpStorageConfiguationId.ToString().ToLower() == input.BackupStorageConfigId.ToLower()  
                    && i.BackupLogStatus == BackupLogStatus.Success);

                var pagedEntities = await query
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToListAsync();
                var totalCount = query.Count();

                return new PagedResultDto<BackUpLogDto>
                {
                    TotalCount = totalCount,
                    Items = ObjectMapper.Map<List<BackUpLogDto>>(pagedEntities)
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #region DB Backup

        private async Task<(bool, string)> MSSQLBackUp(string ServerIP, string DBInitialCatalog, string DbUsername, string DbPassword, string BackUpInitiatedPath, string DatabaseName)
        {
            var res = "";
            string connectionString = $"Server={ServerIP};Initial Catalog={DBInitialCatalog};Persist Security Info=False;User ID={DbUsername};Password={DbPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string backupFilePath = $@"/{BackUpInitiatedPath}/BackupFiles_{timestamp}.bak";
            string backupQuery = $"BACKUP DATABASE [{DatabaseName}] TO DISK = '{backupFilePath}' WITH FORMAT, MEDIANAME = 'SQLServerBackups', NAME = 'Full Backup of {DatabaseName}';";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(backupQuery, connection))
                    {
                        //command.ExecuteNonQuery();
                        res = "Database backup completed successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Error: " + ex.Message);
            }

            return (true, $@"BackupFiles_{timestamp}.bak");
        }

        private async Task<(bool, string)> PostGresDBBackUp(string BackUpInitiatedPath, string SshUserName, string SshPassword, string ServerIP, string DbPassword, string DbUsername, string DatabaseName)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string backupFileName = $"PostgresBackup_{timestamp}.backup";
            string remoteBackupPath = $"/{BackUpInitiatedPath}/{backupFileName}";

            try
            {
                var authMethod = new PasswordAuthenticationMethod(SshUserName, SshPassword);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(ServerIP, 22, SshUserName, authMethod)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                using (var client = new SshClient(connectionInfo))
                {
                    client.Connect();

                    if (!client.IsConnected)
                    {
                        throw new UserFriendlyException("SSH connection failed.");
                    }

                    string command = $"PGPASSWORD='{DbPassword}' pg_dump -U {DbUsername} -h localhost -p 5432 -F c -b -v -f \"{remoteBackupPath}\" {DatabaseName}";
                    var cmd = client.CreateCommand(command);
                    var result = cmd.Execute();

                    client.Disconnect();
                }
;
                return (true,backupFileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"Error: {ex.Message}");
            }
        }
        #endregion

        #region Application Backup
        private async Task<(bool res, string bName)> ApplicationBackup(string Sourcepath, string BackUpInitiatedPath, string PrivateKeyPath, string SshUserName, string ServerIP, string SshPassword, string localPath)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string remoteFolderPath = $@"/{Sourcepath}/";
            string remoteZipPath = $"/{BackUpInitiatedPath}/BackupFile_{timestamp}.zip";
            string fileName = $"BackupAppFile_{timestamp}.zip";
            string localFilePath = Path.Combine(localPath, fileName);

            if (PrivateKeyPath != null)
            {
                try
                {
                    if (!File.Exists(PrivateKeyPath))
                    {
                        throw new FileNotFoundException($"Private key file not found at: {PrivateKeyPath}");
                    }
                    using (var keyStream = new FileStream(PrivateKeyPath, FileMode.Open, FileAccess.Read))
                    {
                        var privateKey = new PrivateKeyFile(keyStream);
                        var authMethod = new PrivateKeyAuthenticationMethod(SshUserName, privateKey);

                        var connectionInfo = new Renci.SshNet.ConnectionInfo(ServerIP, SshUserName, authMethod)
                        {
                            Timeout = TimeSpan.FromSeconds(30)
                        };

                        using (var client = new SshClient(connectionInfo))
                        using (var sftp = new SftpClient(connectionInfo))
                        {
                            client.Connect();

                            if (!client.IsConnected)
                            {
                                throw new Exception("SSH connection failed.");
                            }

                            // Include from the last directory of the remoteFolderPath
                            remoteFolderPath = remoteFolderPath.TrimEnd('/');
                            string parentFolder = remoteFolderPath.Substring(0, remoteFolderPath.LastIndexOf('/'));
                            string targetFolder = remoteFolderPath.Substring(remoteFolderPath.LastIndexOf('/') + 1);
                            if (string.IsNullOrEmpty(parentFolder) || string.IsNullOrEmpty(targetFolder))
                            {
                                throw new Exception("Invalid remote folder path");
                            }
                            var command = $"cd {parentFolder} && zip -r {remoteZipPath} {targetFolder}";
                            var cmd = client.CreateCommand(command);

                            var result = cmd.Execute();

                            // Capture error if any
                            if (!string.IsNullOrEmpty(cmd.Error))
                            {
                                throw new Exception($"Error creating zip file: {cmd.Error}");
                            }

                            sftp.Connect();

                            if (!sftp.Exists(remoteZipPath))
                            {
                                throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipPath}");
                            }
                            var dir = Path.GetDirectoryName(localFilePath);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }

                            // Download zip file to local machine
                            using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                            {
                                sftp.DownloadFile(remoteZipPath, fileStream);
                            }

                            // Delete zip file from remote server after download
                            sftp.DeleteFile(remoteZipPath);

                            sftp.Disconnect();
                            client.Disconnect();
                        }
                    }
                    return(true, fileName);
                }
                catch (FileNotFoundException fnfEx)
                {
                    Console.WriteLine($"File Not Found Error: {fnfEx.Message}");
                    throw new UserFriendlyException($"File not found: {fnfEx.Message}");
                }
                catch (SshAuthenticationException authEx)
                {
                    Console.WriteLine($"SSH Authentication Error: {authEx.Message}");
                    throw new UserFriendlyException($"SSH Authentication failed: {authEx.Message}");
                }
                catch (SshConnectionException connEx)
                {
                    Console.WriteLine($"SSH Connection Error: {connEx.Message}");
                    throw new UserFriendlyException($"SSH Connection failed: {connEx.Message}");
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"I/O Error: {ioEx.Message}");
                    throw new UserFriendlyException($"I/O error occurred: {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    throw new UserFriendlyException($"An unexpected error occurred: {ex.Message}");
                }
            }
            else
            {

                try
                {
                    // Create authentication method using username and password
                    var authMethod = new PasswordAuthenticationMethod(SshUserName, SshPassword);

                    var connectionInfo = new Renci.SshNet.ConnectionInfo(ServerIP, 22, SshUserName, authMethod)
                    {
                        Timeout = TimeSpan.FromSeconds(30)
                    };
                    using (var client = new SshClient(connectionInfo))
                    using (var sftp = new SftpClient(connectionInfo))
                    {
                        // Connect SSH client
                        client.Connect();
                        if (!client.IsConnected)
                        {
                            throw new Exception("SSH connection failed.");
                        }
                        remoteFolderPath = remoteFolderPath.TrimEnd('/');
                        string parentFolder = remoteFolderPath.Substring(0, remoteFolderPath.LastIndexOf('/'));
                        string targetFolder = remoteFolderPath.Substring(remoteFolderPath.LastIndexOf('/') + 1);
                        if (string.IsNullOrEmpty(parentFolder) || string.IsNullOrEmpty(targetFolder))
                        {
                            throw new Exception("Invalid remote folder path");
                        }
                        var command = $"cd {parentFolder} && zip -r {remoteZipPath} {targetFolder}";
                        //var command = $"zip -r {remoteZipPath} {remoteFolderPath}";
                        var cmd = client.CreateCommand(command);
                        var result = cmd.Execute();

                        // Connect SFTP client
                        sftp.Connect();

                        if (!sftp.Exists(remoteZipPath))
                        {
                            throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipPath}");
                        }

                        var dir = Path.GetDirectoryName(localFilePath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        // Download zip file to local machine
                        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                        {
                            sftp.DownloadFile(remoteZipPath, fileStream);
                        }

                        // Delete zip file from remote server after download
                        sftp.DeleteFile(remoteZipPath);

                        // Close connections
                        sftp.Disconnect();
                        client.Disconnect();
                    }

                    return (true, fileName);
                }
                catch (FileNotFoundException fnfEx)
                {
                    Console.WriteLine($"File Not Found Error: {fnfEx.Message}");
                    throw new Exception($"File not found: {fnfEx.Message}");
                }
                catch (SshAuthenticationException authEx)
                {
                    Console.WriteLine($"SSH Authentication Error: {authEx.Message}");
                    throw new Exception($"SSH Authentication failed: {authEx.Message}");
                }
                catch (SshConnectionException connEx)
                {
                    Console.WriteLine($"SSH Connection Error: {connEx.Message}");
                    throw new Exception($"SSH Connection failed: {connEx.Message}");
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"I/O Error: {ioEx.Message}");
                    throw new Exception($"I/O error occurred: {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    throw new Exception($"An unexpected error occurred: {ex.Message}");
                }
            }
        }
        
        #endregion


        #region Handle created Backup files
        private async Task<bool> ZipAndDownloadBackupSSHToLocal(string remoteServerIp, string sshUsername, string sshPassword, string remoteFolderPath, string remoteZipFileWithPath, string localFilePath, string fileName, string PrivateKeyPath)
        {
            string remoteFile = Path.Combine(remoteFolderPath, fileName);

            try
            {
                if(!string.IsNullOrEmpty(PrivateKeyPath))
                {
                    // Create authentication method using username and PrivateKeyFile.
                    var keyFile = new PrivateKeyFile(PrivateKeyPath);
                    var authMethod = new PrivateKeyAuthenticationMethod(sshUsername, keyFile);

                    var connectionInfo = new ConnectionInfo(remoteServerIp, 22, sshUsername, authMethod)
                    {
                        Timeout = TimeSpan.FromSeconds(30)
                    };

                    using (var client = new SshClient(connectionInfo))
                    using (var sftp = new SftpClient(connectionInfo))
                    {
                        // Connect SSH client
                        client.Connect();
                        if (!client.IsConnected)
                        {
                            throw new Exception("SSH connection failed.");
                        }

                        //var command = $"zip -r {remoteZipFileWithPath} {remoteFile}";
                        var outputZipName = Path.GetFileName(remoteZipFileWithPath);
                        var command = $"cd {remoteFolderPath} && zip {outputZipName} {fileName}";
                        var cmd = client.CreateCommand(command);
                        var result = cmd.Execute();

                        // Connect SFTP client
                        sftp.Connect();

                        if (!sftp.Exists(remoteZipFileWithPath))
                        {
                            throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipFileWithPath}");
                        }

                        var dir = Path.GetDirectoryName(localFilePath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        // Download zip file to local machine
                        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                        {
                            sftp.DownloadFile(remoteZipFileWithPath, fileStream);
                        }

                        // Delete zip file from remote server after download
                        sftp.DeleteFile(remoteZipFileWithPath);

                        // Close connections
                        sftp.Disconnect();
                        client.Disconnect();
                    }
                }
                else
                {
                    // Create authentication method using username and password
                    var authMethod = new PasswordAuthenticationMethod(sshUsername, sshPassword);
                    var connectionInfo = new ConnectionInfo(remoteServerIp, 22, sshUsername, authMethod)
                    {
                        Timeout = TimeSpan.FromSeconds(30)
                    };

                    using (var client = new SshClient(connectionInfo))
                    using (var sftp = new SftpClient(connectionInfo))
                    {
                        // Connect SSH client
                        client.Connect();
                        if (!client.IsConnected)
                        {
                            throw new Exception("SSH connection failed.");
                        }
                        //var command = $"zip -r {remoteZipFileWithPath} {remoteFile}";
                        var outputZipName = Path.GetFileName(remoteZipFileWithPath);
                        var command = $"cd {remoteFolderPath} && zip {outputZipName} {fileName}";
                        var cmd = client.CreateCommand(command);
                        var result = cmd.Execute();

                        sftp.Connect();

                        if (!sftp.Exists(remoteZipFileWithPath))
                        {
                            throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipFileWithPath}");
                        }

                        var dir = Path.GetDirectoryName(localFilePath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        // Download zip file to local machine
                        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                        {
                            sftp.DownloadFile(remoteZipFileWithPath, fileStream);
                        }

                        // Delete zip file from remote server after download
                        sftp.DeleteFile(remoteZipFileWithPath);

                        // Close connections
                        sftp.Disconnect();
                        client.Disconnect();
                    }
                }
                

                return true ;
            }
            catch (FileNotFoundException fnfEx)
            {
                Console.WriteLine($"File Not Found Error: {fnfEx.Message}");
                throw new Exception($"File not found: {fnfEx.Message}");
            }
            catch (SshAuthenticationException authEx)
            {
                Console.WriteLine($"SSH Authentication Error: {authEx.Message}");
                throw new Exception($"SSH Authentication failed: {authEx.Message}");
            }
            catch (SshConnectionException connEx)
            {
                Console.WriteLine($"SSH Connection Error: {connEx.Message}");
                throw new Exception($"SSH Connection failed: {connEx.Message}");
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"I/O Error: {ioEx.Message}");
                throw new Exception($"I/O error occurred: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }
                        
        private async Task<string> UploadFileToS3(string awsAccessKey, string awsSecretKey, string s3BucketName, string s3Key, string systemLocalPath)
        {
            try
            {
                using (var s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.APSouth1))
                {
                    var fileTransferUtility = new TransferUtility(s3Client);

                    // Upload file
                    await fileTransferUtility.UploadAsync(systemLocalPath, s3BucketName, s3Key);
                }

                return $"File uploaded successfully to S3: s3://{s3BucketName}/{s3Key}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        #endregion

        private async Task<(bool, Guid)> CreatebackLog(Guid sourceConfiguationId, DateTime startedTimeStamp, Guid backUpStorageConfiguationId, int tenantId)
        {
            try
            {
                Guid backUpLogId;
                var backUpLog = new BackUpLog
                {
                    TenantId = tenantId,
                    SourceConfiguationId = sourceConfiguationId,
                    StartedTimeStamp = startedTimeStamp,
                    BackUpStorageConfiguationId = backUpStorageConfiguationId,
                    BackupLogStatus = BackupLogStatus.Initiated,
                };

                using (CurrentUnitOfWork.SetTenantId(tenantId))
                {
                    backUpLogId = await _backUpLogsRepository.InsertAndGetIdAsync(backUpLog);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                return (true, backUpLogId);
            }
            catch (Exception ex)
            {
                // You may want to log this
                return (false, Guid.Empty);
            }
        }


        private async Task<bool> UpdatebackLogStatus(Guid backUpLogId, DateTime completedTimeStamp, BackupLogStatus status, string bFileName, string bFilePath)
        {
            try
            {
                var getBackUpLog = await _backUpLogsRepository.FirstOrDefaultAsync(i => i.Id == backUpLogId);
                if (getBackUpLog == null)
                {
                    throw new UserFriendlyException("Invalid backup log id");
                }

                getBackUpLog.CompletedTimeStamp = completedTimeStamp;
                getBackUpLog.BackupLogStatus = (BackupLogStatus)status;
                getBackUpLog.BackupFilPath = bFilePath;
                getBackUpLog.BackUpFileName = bFileName;

                using (CurrentUnitOfWork.SetTenantId(getBackUpLog.TenantId))
                {
                    await _backUpLogsRepository.InsertOrUpdateAndGetIdAsync(getBackUpLog);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex) { }
            return true;
        }

        [RemoteService(false)]
        public async Task<Tuple<Stream, string, string>> DownloadBackupStreamAsync(string? sourceConfigurationId, string? storageCongigurationId, string backUpFileName)
        {
            BackUpStorageConfiguation storageConfig = null;

            if (!string.IsNullOrWhiteSpace(sourceConfigurationId))
            {
                var sourceConfig = await _sourceConfiguationRepository.GetAll()
                    .Include(i => i.BackUpStorageConfiguation)
                        .ThenInclude(e => e.StorageMasterType)
                    .Include(i => i.BackUpStorageConfiguation)
                        .ThenInclude(e => e.CloudStorage)
                    .FirstOrDefaultAsync(i => i.Id.ToString().ToLower() == sourceConfigurationId.ToLower());

                if (sourceConfig == null)
                    throw new UserFriendlyException($"Source configuration with ID {sourceConfigurationId} not found");

                if (sourceConfig.BackUpStorageConfiguation == null)
                    throw new UserFriendlyException("Backup storage configuration not found in source config");

                storageConfig = sourceConfig.BackUpStorageConfiguation;
            }
            else if (!string.IsNullOrWhiteSpace(storageCongigurationId))
            {
                storageConfig = await _backUpStorageConfiguationRepository.GetAll()
                    .Include(x => x.CloudStorage)
                    .Include(x => x.StorageMasterType)
                    .FirstOrDefaultAsync(x => x.Id.ToString().ToLower() == storageCongigurationId.ToLower());

                if (storageConfig == null)
                    throw new UserFriendlyException($"Backup storage configuration with ID {storageCongigurationId} not found");
            }
            else
            {
                throw new UserFriendlyException("Either sourceConfigurationId or storageCongigurationId must be provided");
            }

            string storageType = storageConfig.StorageMasterType?.Name;
            string cloudStorageType = storageConfig.CloudStorage?.Name;

            switch (storageType)
            {
                case "Public Cloud":
                    switch (cloudStorageType)
                    {
                        case "Amazon S3":
                            return await DownloadFromAwsS3Async(
                                storageConfig.AWS_AccessKey,
                                storageConfig.AWS_SecretKey,
                                storageConfig.AWS_Region,
                                storageConfig.AWS_backUpPath,
                                backUpFileName,
                                storageConfig.AWS_BucketName);

                        default:
                            throw new NotSupportedException($"Cloud storage type {cloudStorageType} is not implemented");
                    }

                default:
                    throw new NotSupportedException($"Storage type {storageType} is not implemented");
            }
        }

        private async Task<Tuple<Stream, string, string>> DownloadFromAwsS3Async(
            string awsAccessKey,
            string awsSecretKey,
            string awsRegion,
            string backupPath,
            string backUpFileName,
            string bucketName)
        {
            try
            {
                using var s3Client = new AmazonS3Client(
                    awsAccessKey,
                    awsSecretKey,
                    RegionEndpoint.GetBySystemName(awsRegion));

                var s3Key = string.IsNullOrEmpty(backupPath)
                    ? backUpFileName
                    : $"{backupPath.TrimEnd('/')}/{backUpFileName}";

                var metadataRequest = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = s3Key
                };

                var metadata = await s3Client.GetObjectMetadataAsync(metadataRequest);

                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = s3Key
                };

                var response = await s3Client.GetObjectAsync(request);

                var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var contentType = metadata.Headers.ContentType ?? "application/octet-stream";

                return new Tuple<Stream, string, string>(memoryStream, contentType, backUpFileName);
            }
            catch (Exception ex)
            {
                Logger.Error("Download failed", ex);
                throw new UserFriendlyException("An error occurred while downloading the backup");
            }
        }

        public async Task<DashBoardItemDto> GetDashBoardItem()
        {
            try
            {
                int tenantId = GetCurrentTenantId();
                var sourceBackCount = await _sourceConfiguationRepository.GetAll().Where(x => x.TenantId == tenantId).CountAsync();
                var configuredBackStorageCount = await _backUpStorageConfiguationRepository.GetAll().Where(x => x.TenantId == tenantId && !x.IsDeleted && !x.IsUserLocalSystem).CountAsync();
                var totalSchedule = await _backUpScheduleRepository.GetAll().Where(x => x.TenantId == tenantId).CountAsync();
                var totalBackupTaken = await _backUpLogsRepository.GetAll().Where(x => x.TenantId == tenantId && x.BackupLogStatus == BackupLogStatus.Success).CountAsync();
                var lastBackUp = await _backUpLogsRepository.GetAllIncluding(e => e.SourceConfiguation).Where(x => x.TenantId == tenantId && x.BackupLogStatus == BackupLogStatus.Success).OrderByDescending(x => x.CompletedTimeStamp).FirstOrDefaultAsync();

                var nextScheduleList = await _backUpScheduleRepository.GetAllIncluding(e => e.SourceConfiguation)
                    .Where(x => x.TenantId == tenantId && !x.IsRemoveFromHangfire).Select(x => new { x.SourceConfiguation, x.CronExpression }).Distinct().ToListAsync();

                var response = new DashBoardItemDto
                {
                    BackupSourceCount = sourceBackCount,
                    BackupStorageCount = configuredBackStorageCount,
                    ScheduleBackupCount = totalSchedule,
                    TotalBackupCount = totalBackupTaken,
                    LastBackupItem =lastBackUp!=null?lastBackUp.SourceConfiguation.BackupName ??
                        (lastBackUp.SourceConfiguation.BackUPType.BackupTypeEnum == BackupTypeEnum.DataBase ? (lastBackUp.SourceConfiguation.ServerIP + " - " + lastBackUp.SourceConfiguation.DBInitialCatalog)
                        : lastBackUp.SourceConfiguation.ServerIP + " - Application Files"):"",
                    LastBackupStatus = lastBackUp != null ? lastBackUp.BackupLogStatus.ToString():"",
                };

                foreach (var item in nextScheduleList)
                {
                    var sItem = new Sconfig();
                    sItem.Name = item.SourceConfiguation.BackupName ??
                    (item.SourceConfiguation.BackUPType.BackupTypeEnum == BackupTypeEnum.DataBase ? (item.SourceConfiguation.ServerIP + " - " + item.SourceConfiguation.DBInitialCatalog)
                    : item.SourceConfiguation.ServerIP + " - Application Files");
                    
                    if (!String.IsNullOrEmpty(item.CronExpression))
                    {
                        sItem.CronExo = await ConvertCronExpToRegularValue(item.CronExpression);
                    }

                    response.NextScheduleList.Add(sItem);
                }

                return response;
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<string> ConvertCronExpToRegularValue(string cronExp)
        {
            var options = new Options
            {
                Locale = "en", // or "fr", "de", etc.
                Use24HourTimeFormat = false
            };

            var description = ExpressionDescriptor.GetDescription(cronExp, options);

            return description;
        }

    }
}
