using Abp.Domain.Entities;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.MultiTenancy;
using GeekathonAutoSync.SourceConfiguations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekathonAutoSync.BackUpLogs
{
    public class BackUpLog : Entity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public Guid? SourceConfiguationId { get; set; }
        [ForeignKey("SourceConfiguationId")]
        public virtual SourceConfiguation SourceConfiguation { get; set; }
        public DateTime? StartedTimeStamp { get; set; }
        public DateTime? CompletedTimeStamp { get; set; }
        public BackupLogStatus? BackupLogStatus { get; set; }
        public string BackUpFileName { get; set; }
        public string BackupFilPath { get; set; }
        public Guid? BackUpStorageConfiguationId { get; set; }
        [ForeignKey("BackUpStorageConfiguationId")]
        public virtual BackUpStorageConfiguation BackUpStorageConfiguation { get; set; }
    }
}
