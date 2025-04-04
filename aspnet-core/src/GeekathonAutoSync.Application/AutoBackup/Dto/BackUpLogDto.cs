using Abp.Domain.Entities;
using GeekathonAutoSync.BackUpLogs;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.MultiTenancy;
using GeekathonAutoSync.SourceConfiguations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup.Dto
{
    public class BackUpLogDto
    {
            public int TenantId { get; set; }
            public Tenant Tenant { get; set; }
            public Guid? SourceConfiguationId { get; set; }
            public SourceConfiguation SourceConfiguation { get; set; }
            public DateTime? StartedTimeStamp { get; set; }
            public DateTime? CompletedTimeStamp { get; set; }
            public BackupLogStatus? BackupLogStatus { get; set; }
            public string BackUpFileName { get; set; }
            public string BackupFilPath { get; set; }
        public Guid? BackUpStorageConfiguationId { get; set; }
            public BackUpStorageConfiguation BackUpStorageConfiguation { get; set; }
    }
}
