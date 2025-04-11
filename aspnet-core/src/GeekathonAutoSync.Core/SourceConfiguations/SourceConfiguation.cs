using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using GeekathonAutoSync.MultiTenancy;
using System.ComponentModel.DataAnnotations.Schema;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.DBTypes;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUpLogs;
using System.Collections.Generic;
using GeekathonAutoSync.BackUpSchedules;

namespace GeekathonAutoSync.SourceConfiguations
{
    public class SourceConfiguation : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public Guid BackUPTypeId { get; set; }
        [ForeignKey("BackUPTypeId")]
        public virtual BackUPType BackUPType { get; set; }
        public Guid? DBTypeId { get; set; }
        [ForeignKey("DBTypeId")]
        public virtual DBType DBType { get; set; }
        public string DatabaseName { get; set; }
        public string DbUsername { get; set; }
        public string DbPassword { get; set; }
        public string ServerIP { get; set; }
        public string DBInitialCatalog { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string PrivateKeyPath { get; set; }
        public string BackUpInitiatedPath { get; set; }
        public string Sourcepath { get; set; }
        public string OS { get; set; }
        public Guid? BackUpStorageConfiguationId { get; set; }
        [ForeignKey("BackUpStorageConfiguationId")]
        public virtual BackUpStorageConfiguation? BackUpStorageConfiguation { get; set; }
        public string Port { get; set; }
        public string SshUserName { get; set; }
        public string SshPassword { get; set; }
        public string BackupName { get; set; }
        public ICollection<BackUpSchedule> BackUpSchedules { get; set; } = new List<BackUpSchedule>();
        public ICollection<BackUpLog> BackUpLogs { get; set; } = new List<BackUpLog>();
    }
}
