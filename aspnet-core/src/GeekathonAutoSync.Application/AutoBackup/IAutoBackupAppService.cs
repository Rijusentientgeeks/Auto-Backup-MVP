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
        Task<PagedResultDto<BackUpLogDto>> GetAllBackupLogAsync(PagedBackLogRequestDto input);
        Task<Tuple<Stream, string, string>> DownloadBackupStreamAsync(string sourceConfigurationId, string backUpFileName);
    }
}
