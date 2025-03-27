using Abp.Application.Services;
using GeekathonAutoSync.SourceConfiguations.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup
{
    public interface IAutoBackupAppService : IApplicationService
    {
        Task<string> Backup(SourceConfiguationCreateDto input, string backupTypeName, string dbTypeName);
    }
}
