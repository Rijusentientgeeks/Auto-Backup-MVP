using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup.Dto
{
    public class PagedBackLogRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public string SourceConfigId { get; set; }
        public string BackupStorageid { get; set; }
        public string BackupTypeId { get; set; }
        public string CloudStorageTypeId { get; set; }
    }
}
