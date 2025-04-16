﻿using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using GeekathonAutoSync.AutoBackup;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.SourceConfiguations;
using GeekathonAutoSync.SpeechRecognitions.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SpeechRecognitions
{
    public class SpeechRecognitionAppService : GeekathonAutoSyncAppServiceBase, ISpeechRecognitionAppService
    {
        private readonly IRepository<SourceConfiguation, Guid> _sourceConfiguationRepository;
        private readonly IRepository<BackUPType, Guid> _backupTypeRepository;
        private readonly IAutoBackupAppService _autoBackupService;
        public SpeechRecognitionAppService(
            IRepository<SourceConfiguation, Guid> sourceConfiguationRepository,
            IRepository<BackUPType, Guid> backupTypeRepository,
            IAutoBackupAppService autoBackupService)
        {
            _sourceConfiguationRepository = sourceConfiguationRepository;
            _backupTypeRepository = backupTypeRepository;
            _autoBackupService = autoBackupService;
        }
        public async Task<CommandResultDto> ReceiveCommandAsync([FromBody] CommandDto model)
        {
            bool IsValidCommand = false;
            string resultDetails = string.Empty;
            var command = model.Command.ToLower();
            Console.WriteLine($"Received command: {command}");

            if (command.Contains("open google"))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.google.com",
                    UseShellExecute = true
                });
                resultDetails = "Your command is '" + command + "', it's successfully executed.";
                IsValidCommand = true;
            }
            else if (command.Contains("open files"))
            {
                Process.Start("explorer.exe");
                resultDetails = "Your command is '" + command + "', it's successfully executed.";
                IsValidCommand = true;
            }
            else if (command.Contains("open downloads"))
            {
                var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                Process.Start("explorer.exe", downloads);
                resultDetails = "Your command is '" + command + "', it's successfully executed.";
                IsValidCommand = true;
            }
            else
            {
                if (command.Contains("application files") || command.Contains("application file") || command.Contains("application"))
                {
                    var backupType = await _backupTypeRepository.FirstOrDefaultAsync(i => i.BackupTypeEnum == BackupTypeEnum.ApplicationFiles);
                    var serverIP = string.Empty;
                    MatchCollection matches = Regex.Matches(command, @"[-+]?[0-9]*\.?[0-9]+");
                    List<string> ipNumber = [];
                    foreach (var item in matches)
                    {
                        string[] parts = item.ToString().Split(new[] { '.' });
                        ipNumber.AddRange(parts);
                    }
                    if(ipNumber.Count == 4)
                    {
                        serverIP = string.Join(".", ipNumber);
                        var getSourceConfiguration = _sourceConfiguationRepository.GetAll().FirstOrDefault(i => i.BackUPTypeId == backupType.Id && i.ServerIP == serverIP);
                        if (getSourceConfiguration != null)
                        {
                            resultDetails = await _autoBackupService.CreateBackup(getSourceConfiguration.Id.ToString());
                            resultDetails = "Your command is '" + command + "', it's successfully executed and backup created successfully";
                            IsValidCommand = true;
                        }
                        else
                        {
                            resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                            IsValidCommand = false;
                        }
                    }
                    else
                    {
                        resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                        IsValidCommand = false;
                    }

                    //string digitsOnly = Regex.Replace(command, @"\D", "");
                    //if(digitsOnly.Length > 10 && digitsOnly.Length < 13)
                    //{
                    //    serverIP = string.Format("{0}.{1}.{2}.{3}",
                    //        digitsOnly.Substring(0, 3),
                    //        digitsOnly.Substring(3, 3),
                    //        digitsOnly.Substring(6, 3),
                    //        digitsOnly.Substring(9, (digitsOnly.Length - 9))
                    //    );
                    //    var getSourceConfiguration = _sourceConfiguationRepository.GetAll().FirstOrDefault(i => i.BackUPTypeId == backupType.Id && i.ServerIP == serverIP);
                    //    if (getSourceConfiguration != null)
                    //    {
                    //        resultDetails = await _autoBackupService.CreateBackup(getSourceConfiguration.Id.ToString());
                    //        resultDetails = "Your command is '" + command + "', it's successfully executed and backup created successfully";
                    //        IsValidCommand = true;
                    //    }
                    //    else
                    //    {
                    //        resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                    //        IsValidCommand = false;
                    //    }
                    //}
                    //else
                    //{
                    //    resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                    //    IsValidCommand = false;
                    //}
                }
                else if (command.Contains("database"))
                {
                    var backupType = await _backupTypeRepository.FirstOrDefaultAsync(i => i.BackupTypeEnum == BackupTypeEnum.DataBase);
                    var serverIP = string.Empty;
                    MatchCollection matches = Regex.Matches(command, @"[-+]?[0-9]*\.?[0-9]+");
                    List<string> ipNumber = [];
                    foreach (var item in matches)
                    {
                        string[] parts = item.ToString().Split(new[] { '.' });
                        ipNumber.AddRange(parts);
                    }
                    if (ipNumber.Count == 4)
                    {
                        serverIP = string.Join(".", ipNumber);
                        var getSourceConfiguration = _sourceConfiguationRepository.GetAll().FirstOrDefault(i => i.BackUPTypeId == backupType.Id && i.ServerIP == serverIP);
                        if (getSourceConfiguration != null)
                        {
                            resultDetails = await _autoBackupService.CreateBackup(getSourceConfiguration.Id.ToString());
                            resultDetails = "Your command is '" + command + "', it's successfully executed and backup created successfully";
                            IsValidCommand = true;
                        }
                        else
                        {
                            resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                            IsValidCommand = false;
                        }
                    }
                    else
                    {
                        resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                        IsValidCommand = false;
                    }
                    //string digitsOnly = Regex.Replace(command, @"\D", "");
                    //var serverIP = string.Empty;
                    //if (digitsOnly.Length > 10 && digitsOnly.Length < 13)
                    //{
                    //    serverIP = string.Format("{0}.{1}.{2}.{3}",
                    //        digitsOnly.Substring(0, 3),
                    //        digitsOnly.Substring(3, 3),
                    //        digitsOnly.Substring(6, 3),
                    //        digitsOnly.Substring(9, (digitsOnly.Length - 9))
                    //    );
                    //    var getSourceConfiguration = _sourceConfiguationRepository.GetAll().FirstOrDefault(i => i.BackUPTypeId == backupType.Id && i.ServerIP == serverIP);
                    //    if (getSourceConfiguration != null)
                    //    {
                    //        resultDetails = await _autoBackupService.CreateBackup(getSourceConfiguration.Id.ToString());
                    //        IsValidCommand = true;
                    //    }
                    //    else
                    //    {
                    //        resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                    //        IsValidCommand = false;
                    //    }
                    //}
                    //else
                    //{
                    //    resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                    //    IsValidCommand = false;
                    //}
                }
                else
                {
                    resultDetails = "Your command is '" + command + "', it's a invalid command, so we cann't execute it.";
                    IsValidCommand = false;
                }
                
            }
            var resultDto = new CommandResultDto();
            resultDto.Result = resultDetails;
            resultDto.IsValidCommand = IsValidCommand;
            return resultDto;
        }
    }
}
