using Abp.UI;
using GeekathonAutoSync.SourceConfiguations.Dto;
using Microsoft.Data.SqlClient;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup
{
    public class AutoBackupAppService : GeekathonAutoSyncAppServiceBase, IAutoBackupAppService
    {

        public AutoBackupAppService()
        {
            
        }
        public async Task<string> Backup(SourceConfiguationCreateDto input, string backupTypeName, string dbTypeName)
        {
            var res = "";
            switch (backupTypeName)
            {
                case "DataBase":
                    switch (dbTypeName)
                    {
                        case "PostgreSQL":
                            res = await PostGresDBBackUp(input);
                            break;
                        case "Microsoft SQL Server":
                            res = await MSSQLBackUp(input);
                            break;
                        case "Oracle Database":
                            res = "Work Inprogress.";
                            break;
                        case "MySQL":
                            res = "Work Inprogress.";
                            break;
                        case "MongoDB":
                            res = "Work Inprogress.";
                            break;
                        default:
                            res = dbTypeName + " database type is not valid.";
                            break;
                    }
                    break;
                case "Application Files":
                    res = await ApplicationBackup(input);
                    break;
                case "Specific File":
                    break;
                default:
                    break;
            }
            return res;
        }

        #region DB Backup
        public async Task<string> MSSQLBackUp(SourceConfiguationCreateDto input)
        {
            var res = "";
            //string connectionString = "Server=34.240.155.123;Initial Catalog=PolymerConnectDB;Persist Security Info=False;User ID=sa;Password=Geeks$456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
            string connectionString = $"Server={input.ServerIP};Initial Catalog={input.DBInitialCatalog};Persist Security Info=False;User ID={input.DbUsername};Password={input.DbPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
            //string backupFilePath = @"C:/Backup/TestBackUp.bak";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            //string backupFilePath = @"/var/opt/mssql/data/TestBackUp.bak";
            string backupFilePath = $@"/{input.BackUpInitiatedPath}/BackupFiles_{timestamp}.bak";
            //string backupFilePath = @"/home/ubuntu/TestBackUpMsSQL.bak";
            //string databaseName = "PolymerConnectDB";
            //string databaseName = database_Name;

            string backupQuery = $"BACKUP DATABASE [{input.DatabaseName}] TO DISK = '{backupFilePath}' WITH FORMAT, MEDIANAME = 'SQLServerBackups', NAME = 'Full Backup of {input.DatabaseName}';";
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

            return res;
        }

        public async Task<string> PostGresDBBackUp(SourceConfiguationCreateDto input)
        {
            //string remoteServerIp = "184.174.33.208";//source
            //string username = "asim"; // SSH username//source
            //string sshPassword = "asim"; // SSH password//source
            //string databaseName = "NGCOrderingSystem";//source
            //string dbUsername = "postgres";//source
            //string dbPassword = "sgeeks@2024";//source
            //string backupFileName = $"PostgresBackup.backup";//add timespan
            //string remoteBackupPath = $"/var/www/html/NGCOrderingSystem/dbbackup/{backupFileName}";//source
            //string localBackupPath = $@"D:\DOT NET\AutoSync-SG\backup\{backupFileName}";
            
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string backupFileName = $"PostgresBackup_{timestamp}.backup";
            string remoteBackupPath = $"/{input.BackUpInitiatedPath}/{backupFileName}";
            string localBackupPath = $@"D:\DOT NET\AutoSync-SG\backup\{backupFileName}";

            try
            {
                var authMethod = new PasswordAuthenticationMethod(input.SshUserName, input.SshPassword);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(input.ServerIP, 22, input.SshUserName, authMethod)
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
                    string command = $"PGPASSWORD='{input.DbPassword}' pg_dump -U {input.DbUsername} -h localhost -p 5432 -F c -b -v -f \"{remoteBackupPath}\" {input.DatabaseName}";
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
                return $"Backup file downloaded successfully to {localBackupPath}";
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error: {ex.Message}");
                throw new UserFriendlyException($"Error: {ex.Message}");
            }
        }
        #endregion

        #region Application Backup
        public async Task<string> ApplicationBackup(SourceConfiguationCreateDto input)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string remoteFolderPath = $@"/{input.Sourcepath}/";
            string remoteZipPath = $"/{input.BackUpInitiatedPath}/BackupFile_{timestamp}.zip";
            //string localFilePath = "";

            if(input.PrivateKeyPath != null)
            {
                string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\BackupFiles_{timestamp}.zip";
                try
                {
                    if (!File.Exists(input.PrivateKeyPath))
                    {
                        throw new FileNotFoundException($"Private key file not found at: {input.PrivateKeyPath}");
                    }
                    using (var keyStream = new FileStream(input.PrivateKeyPath, FileMode.Open, FileAccess.Read))
                    {
                        var privateKey = new PrivateKeyFile(keyStream);
                        var authMethod = new PrivateKeyAuthenticationMethod(input.SshUserName, privateKey);

                        var connectionInfo = new Renci.SshNet.ConnectionInfo(input.ServerIP, input.SshUserName, authMethod)
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
                    return "Backup files zipped and downloaded successfully.";
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
                string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\QuantamoApplicationBackup_{timestamp}.zip";
                try
                {
                    // Create authentication method using username and password
                    var authMethod = new PasswordAuthenticationMethod(input.SshUserName, input.SshPassword);

                    var connectionInfo = new Renci.SshNet.ConnectionInfo(input.ServerIP, 22, input.SshUserName, authMethod)
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

                    return "Backup files zipped and downloaded successfully.";
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
        //public async Task<string> ZipAndDownloadRemoteFolder(string serverIP, string sshUserName, 
        //    string privateKeyPath, string sourcePath, string backupInitiatedPath)
        //{
        //    //string remoteServerIp = "34.240.155.123";//source
        //    //string username = "ubuntu";//SSH user name source
        //    //string privateKeyPath = @"D:\SecretKey\secretKey.pem";//source
        //    ////string remoteFolderPath = @"/var/opt/mssql/data";
        //    //string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        //    //string remoteFolderPath = @"/var/www/html/polymer/polymer-connect-backend/PolymerConnectWebAPI/api/";//source path
        //    //string remoteZipPath = $"/home/ubuntu/BackupFiles_{timestamp}.zip";//source store backup int
        //    //string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\BackupFiles_{timestamp}.zip";//destination
        //    ////string localFilePath = $"D:\\DOT NET\\AutoSync-SG\\backup\\BackupFiles_{timestamp}.zip";

        //    string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        //    string remoteFolderPath = $@"/{sourcePath}/";
        //    string remoteZipPath = $"/{backupInitiatedPath}/BackupFile_{timestamp}.zip";
        //    string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\BackupFiles_{timestamp}.zip";

        //    try
        //    {
        //        if (!System.IO.File.Exists(privateKeyPath))
        //        {
        //            throw new FileNotFoundException($"Private key file not found at: {privateKeyPath}");
        //        }

        //        using (var keyStream = new FileStream(privateKeyPath, FileMode.Open, FileAccess.Read))
        //        {
        //            var privateKey = new PrivateKeyFile(keyStream);
        //            var authMethod = new PrivateKeyAuthenticationMethod(sshUserName, privateKey);

        //            var connectionInfo = new Renci.SshNet.ConnectionInfo(serverIP, sshUserName, authMethod)
        //            {
        //                Timeout = TimeSpan.FromSeconds(30)
        //            };

        //            using (var client = new SshClient(connectionInfo))
        //            using (var sftp = new SftpClient(connectionInfo))
        //            {
        //                client.Connect();

        //                if (!client.IsConnected)
        //                {
        //                    throw new Exception("SSH connection failed.");
        //                }

        //                // Create zip file on remote server (Include the entire path in the Zip)
        //                //var command = $"zip -r {remoteZipPath} {remoteFolderPath}";
        //                //var cmd = client.CreateCommand(command);


        //                // Include from the last directory of the remoteFolderPath
        //                remoteFolderPath = remoteFolderPath.TrimEnd('/');
        //                string parentFolder = remoteFolderPath.Substring(0, remoteFolderPath.LastIndexOf('/'));
        //                string targetFolder = remoteFolderPath.Substring(remoteFolderPath.LastIndexOf('/') + 1);
        //                if (string.IsNullOrEmpty(parentFolder) || string.IsNullOrEmpty(targetFolder))
        //                {
        //                    throw new Exception("Invalid remote folder path");
        //                }
        //                var command = $"cd {parentFolder} && zip -r {remoteZipPath} {targetFolder}";
        //                var cmd = client.CreateCommand(command);

        //                var result = cmd.Execute();

        //                // Capture error if any
        //                if (!string.IsNullOrEmpty(cmd.Error))
        //                {
        //                    throw new Exception($"Error creating zip file: {cmd.Error}");
        //                }

        //                sftp.Connect();

        //                if (!sftp.Exists(remoteZipPath))
        //                {
        //                    throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipPath}");
        //                }

        //                // Download zip file to local machine
        //                using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
        //                {
        //                    sftp.DownloadFile(remoteZipPath, fileStream);
        //                }

        //                // Delete zip file from remote server after download
        //                sftp.DeleteFile(remoteZipPath);

        //                sftp.Disconnect();
        //                client.Disconnect();
        //            }
        //        }

        //        return "Backup files zipped and downloaded successfully.";
        //    }
        //    catch (FileNotFoundException fnfEx)
        //    {
        //        Console.WriteLine($"File Not Found Error: {fnfEx.Message}");
        //        throw new UserFriendlyException($"File not found: {fnfEx.Message}");
        //    }
        //    catch (SshAuthenticationException authEx)
        //    {
        //        Console.WriteLine($"SSH Authentication Error: {authEx.Message}");
        //        throw new UserFriendlyException($"SSH Authentication failed: {authEx.Message}");
        //    }
        //    catch (SshConnectionException connEx)
        //    {
        //        Console.WriteLine($"SSH Connection Error: {connEx.Message}");
        //        throw new UserFriendlyException($"SSH Connection failed: {connEx.Message}");
        //    }
        //    catch (IOException ioEx)
        //    {
        //        Console.WriteLine($"I/O Error: {ioEx.Message}");
        //        throw new UserFriendlyException($"I/O error occurred: {ioEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"General Error: {ex.Message}");
        //        throw new UserFriendlyException($"An unexpected error occurred: {ex.Message}");
        //    }
        //}

        //public async Task<string> DownloadAndZipRemoteFolderQuantamoServer(string serverIP, string sshUserName,
        //    string sshPassword, string sourcePath, string backupInitiatedPath)
        //{
        //    //string remoteServerIp = "184.174.33.208"; // Update to your server IP
        //    //string username = "asim"; // Your SSH username
        //    //string password = "asim"; // Your SSH password
        //    //string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        //    //string remoteFolderPath = @"/var/www/html/NGCOrderingSystem/aspnet-core/src/NGCOrderingSystem.Web.Host/";
        //    //string remoteZipPath = $"/var/www/html/NGCOrderingSystem/applicationBackup/QuantamoApplicationBackup_{timestamp}.zip";
        //    //string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\QuantamoApplicationBackup_{timestamp}.zip";

        //    string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        //    string remoteFolderPath = $@"/{sourcePath}/";
        //    string remoteZipPath = $"/{backupInitiatedPath}/BackupFile_{timestamp}.zip"; ;
        //    string localFilePath = $@"D:\DOT NET\AutoSync-SG\backup\QuantamoApplicationBackup_{timestamp}.zip";

        //    try
        //    {
        //        // Create authentication method using username and password
        //        var authMethod = new PasswordAuthenticationMethod(sshUserName, sshPassword);

        //        var connectionInfo = new Renci.SshNet.ConnectionInfo(serverIP, 22, sshUserName, authMethod)
        //        {
        //            Timeout = TimeSpan.FromSeconds(30)
        //        };

        //        using (var client = new SshClient(connectionInfo))
        //        using (var sftp = new SftpClient(connectionInfo))
        //        {
        //            // Connect SSH client
        //            client.Connect();
        //            if (!client.IsConnected)
        //            {
        //                throw new Exception("SSH connection failed.");
        //            }

        //            // Prepare folder path for zip command
        //            //remoteFolderPath = remoteFolderPath.TrimEnd('/');
        //            //string parentFolder = remoteFolderPath.Substring(0, remoteFolderPath.LastIndexOf('/'));
        //            //string targetFolder = remoteFolderPath.Substring(remoteFolderPath.LastIndexOf('/') + 1);
        //            //if (string.IsNullOrEmpty(parentFolder) || string.IsNullOrEmpty(targetFolder))
        //            //{
        //            //    throw new Exception("Invalid remote folder path");
        //            //}

        //            //// Zip folder command (include full path)
        //            //var command = $"cd {parentFolder} && zip -r {remoteZipPath} {targetFolder}";
        //            //var cmd = client.CreateCommand(command);
        //            //var result = cmd.Execute();
        //            var command = $"zip -r {remoteZipPath} {remoteFolderPath}";
        //            var cmd = client.CreateCommand(command);
        //            var result = cmd.Execute();

        //            // Connect SFTP client
        //            sftp.Connect();

        //            if (!sftp.Exists(remoteZipPath))
        //            {
        //                throw new FileNotFoundException($"Zip file not found on remote server: {remoteZipPath}");
        //            }

        //            // Download zip file to local machine
        //            using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
        //            {
        //                sftp.DownloadFile(remoteZipPath, fileStream);
        //            }

        //            // Delete zip file from remote server after download
        //            sftp.DeleteFile(remoteZipPath);

        //            // Close connections
        //            sftp.Disconnect();
        //            client.Disconnect();
        //        }

        //        return "Backup files zipped and downloaded successfully.";
        //    }
        //    catch (FileNotFoundException fnfEx)
        //    {
        //        Console.WriteLine($"File Not Found Error: {fnfEx.Message}");
        //        throw new Exception($"File not found: {fnfEx.Message}");
        //    }
        //    catch (SshAuthenticationException authEx)
        //    {
        //        Console.WriteLine($"SSH Authentication Error: {authEx.Message}");
        //        throw new Exception($"SSH Authentication failed: {authEx.Message}");
        //    }
        //    catch (SshConnectionException connEx)
        //    {
        //        Console.WriteLine($"SSH Connection Error: {connEx.Message}");
        //        throw new Exception($"SSH Connection failed: {connEx.Message}");
        //    }
        //    catch (IOException ioEx)
        //    {
        //        Console.WriteLine($"I/O Error: {ioEx.Message}");
        //        throw new Exception($"I/O error occurred: {ioEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"General Error: {ex.Message}");
        //        throw new Exception($"An unexpected error occurred: {ex.Message}");
        //    }
        //}
        #endregion
    }
}
