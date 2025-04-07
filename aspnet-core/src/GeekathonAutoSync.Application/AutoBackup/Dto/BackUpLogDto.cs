using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using GeekathonAutoSync.BackUpLogs;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUpStorageConfiguations.Dto;
using GeekathonAutoSync.MultiTenancy;
using GeekathonAutoSync.SourceConfiguations;
using GeekathonAutoSync.SourceConfiguations.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup.Dto
{
    [AutoMap(typeof(BackUpLog))]
    public class BackUpLogDto : EntityDto<Guid>
    {
            public int TenantId { get; set; }
            public Tenant Tenant { get; set; }
            public Guid? SourceConfiguationId { get; set; }
            public SourceConfiguationDto SourceConfiguation { get; set; }
            public DateTime? StartedTimeStamp { get; set; }
            public DateTime? CompletedTimeStamp { get; set; }
            public BackupLogStatus? BackupLogStatus { get; set; }
            public string BackUpFileName { get; set; }
            public string BackupFilPath { get; set; }
            public Guid? BackUpStorageConfiguationId { get; set; }
            //public BackUpStorageConfiguationDto BackUpStorageConfiguation { get; set; }
    }
}
