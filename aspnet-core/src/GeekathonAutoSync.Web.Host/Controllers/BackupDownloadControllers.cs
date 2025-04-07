﻿using Abp.AspNetCore.Mvc.Controllers;
using GeekathonAutoSync.AutoBackup;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GeekathonAutoSync.Web.Host.Controllers
{
    [Route("api/backup")]
    public class BackupDownloadController : AbpController
    {
        private readonly IAutoBackupAppService _autoBackupAppService;

        public BackupDownloadController(IAutoBackupAppService autoBackupAppService)
        {
            _autoBackupAppService = autoBackupAppService;
        }

        //[HttpGet("download")]
        //public async Task<IActionResult> DownloadBackup([FromQuery] string sourceConfigurationId, [FromQuery] string backUpFileName)
        //{
        //    var result = await _autoBackupAppService.DownloadBackupStreamAsync(sourceConfigurationId, backUpFileName);

        //    var fileStream = result.Item1;
        //    var contentType = result.Item2;
        //    var fileName = result.Item3;

        //    return File(fileStream, contentType, fileName);
        //}

        [HttpGet("download")]
        public async Task<IActionResult> DownloadBackup(string sourceConfigurationId, string storageConfigurationId, string backUpFileName)
        {
            sourceConfigurationId = string.IsNullOrWhiteSpace(sourceConfigurationId) ? null : sourceConfigurationId;
            storageConfigurationId = string.IsNullOrWhiteSpace(storageConfigurationId) ? null : storageConfigurationId;

            var result = await _autoBackupAppService.DownloadBackupStreamAsync(sourceConfigurationId, storageConfigurationId, backUpFileName);

            return File(result.Item1, result.Item2, result.Item3);
        }


    }
}
