using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using GeekathonAutoSync.Authorization.Users;
using GeekathonAutoSync.AutoBackup.Dto;
using GeekathonAutoSync.BackUpLogs;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUpStorageConfiguations.Dto;
using GeekathonAutoSync.CloudStorages;
using GeekathonAutoSync.SourceConfiguations;
using GeekathonAutoSync.SourceConfiguations.Dto;
using GeekathonAutoSync.StorageMasterTypes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GeekathonAutoSync.AutoBackup
{
    public class AutoBackupAppService : GeekathonAutoSyncAppServiceBase, IAutoBackupAppService
    {
        private readonly IRepository<BackUpStorageConfiguation, Guid> _backUpStorageConfiguationRepository;
        private readonly IRepository<StorageMasterType, Guid> _storageMasterTypeRepository;
        private readonly IRepository<CloudStorage, Guid> _cloudStorageRepository;
        private readonly IRepository<SourceConfiguation, Guid> _sourceConfiguationRepository;
        private readonly IRepository<BackUpLog, Guid> _backUpLogsRepository; 
        private readonly IWebHostEnvironment _env;

        private string defaultInitialLocalPath = "LocalBackupFiles";

        public AutoBackupAppService(IRepository<BackUpStorageConfiguation, Guid> backUpStorageConfiguationRepository,
            IRepository<StorageMasterType, Guid> storageMasterTypeRepository,
            IRepository<CloudStorage, Guid> cloudStorageRepository, IRepository<SourceConfiguation, Guid> sourceConfiguationRepository, IWebHostEnvironment env,
            IRepository<BackUpLog, Guid> backUpLogsRepository)
        {
            _backUpStorageConfiguationRepository = backUpStorageConfiguationRepository;
            _storageMasterTypeRepository = storageMasterTypeRepository;
            _cloudStorageRepository = cloudStorageRepository;
            _sourceConfiguationRepository = sourceConfiguationRepository;
            _env = env;
            _backUpLogsRepository = backUpLogsRepository;
        }
        public async Task<string> CreateBackup(string sConfigurationId)
        {

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
            await CreatebackLog(BackUPConfig.Id, DateTime.UtcNow, (Guid)BackUPConfig.BackUpStorageConfiguationId);

            (bool, string) resp = (false, "");
            switch (BackUPConfig.BackUPType.Name)
            {
                case "DataBase":
                    switch (BackUPConfig.DBType.Name)
                    {
                        case "PostgreSQL":
                            resp = await PostGresDBBackUp(BackUPConfig.BackUpInitiatedPath, BackUPConfig.SshUserName, BackUPConfig.SshPassword, BackUPConfig.ServerIP, BackUPConfig.DbPassword, BackUPConfig.DbUsername, BackUPConfig.DatabaseName);
                            break;
                        case "Microsoft SQL Server":
                            resp = await MSSQLBackUp(BackUPConfig.ServerIP, BackUPConfig.DBInitialCatalog, BackUPConfig.DbUsername, BackUPConfig.DbPassword, BackUPConfig.BackUpInitiatedPath, BackUPConfig.DatabaseName);
                            break;
                        case "Oracle Database":
                            resp = (false, "Work Inprogress.");
                            break;
                        case "MySQL":
                            resp = (false, "Work Inprogress.");
                            break;
                        case "MongoDB":
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

                    var response = await ZipAndDownloadBackupSSHToLocal(BackUPConfig.ServerIP, BackUPConfig.SshUserName, BackUPConfig.SshPassword, BackUPConfig.BackUpInitiatedPath, backupZipFileWithPath, serverPath, backupFileName, BackUPConfig.PrivateKeyPath);
                    fileDownloadflag = resp.Item1;
                    downLoadedFileName = $"{nameWithoutExtension}.zip";
                    break;

                case "Application Files":

                    resp = await ApplicationBackup(BackUPConfig.Sourcepath, BackUPConfig.BackUpInitiatedPath, BackUPConfig.PrivateKeyPath, BackUPConfig.DbPassword, BackUPConfig.ServerIP, BackUPConfig.SshPassword, serverPath);
                    downLoadedFileName = resp.Item2;
                    fileDownloadflag = resp.Item1;
                    break;

                case "Specific File":
                    break;
                default:
                    break;
            }



            if (fileDownloadflag == true)
            {
                string storageType = BackUPConfig.BackUpStorageConfiguation.StorageMasterType.Name.ToString();
                string cloudStorageType = BackUPConfig.BackUpStorageConfiguation?.CloudStorage.Name.ToString();

                switch (storageType)
                {
                    case "Public Cloud":
                        switch (cloudStorageType)
                        {
                            case "Amazon S3":
                                string downloadedFileWithPath = Path.Combine(serverPath, downLoadedFileName);
                                resFromUpload = await UploadFileToS3(BackUPConfig.BackUpStorageConfiguation.AWS_AccessKey, BackUPConfig.BackUpStorageConfiguation.AWS_SecretKey, BackUPConfig.BackUpStorageConfiguation.AWS_BucketName, $"/{downLoadedFileName}", $"{downloadedFileWithPath}");
                                break;
                            case "Microsoft Azure":
                                break;
                            case "Google Cloud":
                                break;
                            case "Alibaba Cloud":
                                break;
                            default:
                                break;
                        }

                        break;
                    case "Network File System":
                        break;
                    case "GeekSync Infrastructure Cluster":
                        break;
                    default:
                        break;

                }
            }
            await UpdatebackLogStatus(BackUPConfig.Id, DateTime.UtcNow, BackupLogStatus.Success, downLoadedFileName,"");
            return resFromUpload;
        }

        public async Task<PagedResultDto<BackUpLogDto>> GetAllBackupLogAsync(PagedBackLogRequestDto input)
        {
           
            var query =  _backUpLogsRepository.GetAll()
                .Include(i => i.SourceConfiguation)
                .Include(i => i.BackUpStorageConfiguation)
                .Where(i => i.TenantId == AbpSession.TenantId);

            var filteredQry = query.AsQueryable().WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                    u => u.BackUpFileName.ToLower().Contains(input.Keyword) ||
                         u.SourceConfiguation.BackupName.ToLower().Contains(input.Keyword))
                .WhereIf(!string.IsNullOrEmpty(input.SourceConfigId), u => u.SourceConfiguationId?.ToString().ToLower() == input.SourceConfigId.ToLower())
                .WhereIf(!string.IsNullOrEmpty(input.BackupStorageid), u => u.BackUpStorageConfiguationId?.ToString().ToLower() == input.BackupStorageid.ToLower())
                .WhereIf(!string.IsNullOrEmpty(input.BackupTypeId), u => u.BackUpStorageConfiguation?.StorageMasterTypeId.ToString().ToLower() == input.BackupTypeId.ToLower())
                .WhereIf(!string.IsNullOrEmpty(input.CloudStorageTypeId), u => u.BackUpStorageConfiguation?.CloudStorageId?.ToString().ToLower() == input.CloudStorageTypeId.ToLower())
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();
            var totalCount =  filteredQry.Count();
            return new PagedResultDto<BackUpLogDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BackUpLogDto>>(filteredQry)
            };
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
            string localBackupPath = $@"D:\DOT NET\AutoSync-SG\backup\{backupFileName}";

            try
            {
                var authMethod = new PasswordAuthenticationMethod(SshUserName, SshPassword);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(ServerIP, 22, SshUserName, authMethod)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                using (var client = new SshClient(connectionInfo))
                using (var sftp = new SftpClient(connectionInfo))
                {
                    client.Connect();

                    if (!client.IsConnected)
                    {
                        throw new UserFriendlyException("SSH connection failed.");
                    }

                    // 🏆 Run pg_dump command on the remote server
                    string command = $"PGPASSWORD='{DbPassword}' pg_dump -U {DbUsername} -h localhost -p 5432 -F c -b -v -f \"{remoteBackupPath}\" {DatabaseName}";
                    var cmd = client.CreateCommand(command);
                    var result = cmd.Execute();

                    sftp.Connect();

                    if (!sftp.Exists(remoteBackupPath))
                    {
                        throw new UserFriendlyException($"Backup file not found on remote server: {remoteBackupPath}");
                    }

                    using (var fileStream = new FileStream(localBackupPath, FileMode.Create, FileAccess.Write))
                    {
                        sftp.DownloadFile(remoteBackupPath, fileStream);
                    }

                    //  Delete the remote file after download
                    sftp.DeleteFile(remoteBackupPath);

                    sftp.Disconnect();
                    client.Disconnect();
                }

                //Console.WriteLine($"Backup file downloaded successfully to {localBackupPath}");
                return (true,backupFileName);
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error: {ex.Message}");
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
                //string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\BackupFiles_{timestamp}.zip";
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
                //string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\QuantamoApplicationBackup_{timestamp}.zip";
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

                        var command = $"zip -r {remoteZipPath} {remoteFolderPath}";
                        var cmd = client.CreateCommand(command);
                        var result = cmd.Execute();

                        // Connect SFTP client
                        sftp.Connect();

                        if (!sftp.Exists(remoteZipPath))
                        {
                            throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipPath}");
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

                        var command = $"zip -r {remoteZipFileWithPath} {remoteFile}";
                        var cmd = client.CreateCommand(command);
                        var result = cmd.Execute();

                        // Connect SFTP client
                        sftp.Connect();

                        if (!sftp.Exists(remoteZipFileWithPath))
                        {
                            throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipFileWithPath}");
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

                        var command = $"zip -r {remoteZipFileWithPath} {remoteFile}";
                        var cmd = client.CreateCommand(command);
                        var result = cmd.Execute();

                        // Connect SFTP client
                        sftp.Connect();

                        if (!sftp.Exists(remoteZipFileWithPath))
                        {
                            throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipFileWithPath}");
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


        #region backupLogs
        private async Task<bool> CreatebackLog(Guid sourceConfiguationId, DateTime startedTimeStamp, Guid backUpStorageConfiguationId)
        {
            try
            {
                 var backUpLog = new BackUpLog
                 {
                    TenantId = (int)AbpSession.TenantId,
                     SourceConfiguationId = sourceConfiguationId,
                     StartedTimeStamp = startedTimeStamp,
                     BackUpStorageConfiguationId = backUpStorageConfiguationId,
                     BackupLogStatus = BackupLogStatus.Initiated,
                 };
                using (CurrentUnitOfWork.SetTenantId(backUpLog.TenantId))
                {
                   var backUpLogId = await _backUpLogsRepository.InsertAndGetIdAsync(backUpLog);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex) { }
            return true;
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

        #endregion
    }
}
