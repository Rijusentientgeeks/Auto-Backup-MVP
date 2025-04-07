using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup.Dto
{
    public class PagedBackupLogInputDto : PagedResultRequestDto
    {
        public string BackupStorageConfigId { get; set; }
    }
}
