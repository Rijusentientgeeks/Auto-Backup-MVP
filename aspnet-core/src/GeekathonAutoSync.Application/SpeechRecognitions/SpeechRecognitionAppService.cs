using Abp.Application.Services.Dto;
using GeekathonAutoSync.SpeechRecognitions.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SpeechRecognitions
{
    public class SpeechRecognitionAppService : GeekathonAutoSyncAppServiceBase, ISpeechRecognitionAppService
    {
        //ReceiveCommand
        public async Task<bool> ReceiveCommandAsync([FromBody] CommandDto model)
        {
            bool IsValidCommand = false;
            var command = model.Command.ToLower();
            Console.WriteLine($"Received command: {command}");

            if (command.Contains("open google"))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.google.com",
                    UseShellExecute = true
                });
                IsValidCommand = true;
            }
            else if (command.Contains("open files"))
            {
                Process.Start("explorer.exe");
                IsValidCommand = true;
            }
            else if (command.Contains("open downloads"))
            {
                var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                Process.Start("explorer.exe", downloads);
                IsValidCommand = true;
            }
            else
            {
                // Perform a shutdown or log out logic
                IsValidCommand = false;
            }
            return IsValidCommand;
        }
    }
}
