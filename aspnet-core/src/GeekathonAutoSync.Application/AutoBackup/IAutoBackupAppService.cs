using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeekathonAutoSync.AutoBackup.Dto;
using GeekathonAutoSync.BackUpLogs;
using GeekathonAutoSync.SourceConfiguations.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup
{
    public interface IAutoBackupAppService : IApplicationService
    {
        Task<string> CreateBackup(string sConfigurationId);
        Task<string> CreateBackupAndDownload(string sConfigurationId);
        Task<PagedResultDto<BackUpLogDto>> GetAllBackupLogAsync(PagedBackLogRequestDto input);
        Task<PagedResultDto<BackUpLogDto>> GetAllCompletedBackupLogByStorageConfigIdAsync(PagedBackupLogInputDto input);
        //Task<Tuple<Stream, string, string>> DownloadBackupStreamAsync(string sourceConfigurationId, string backUpFileName);
        Task<Stream> GetBackupFromLocalHost(string filePath);
        Task<Tuple<Stream, string, string>> DownloadBackupStreamAsync(string? sourceConfigurationId, string? storageCongigurationId, string backUpFileName);
        Task<DashBoardItemDto> GetDashBoardItem();
    }
}
