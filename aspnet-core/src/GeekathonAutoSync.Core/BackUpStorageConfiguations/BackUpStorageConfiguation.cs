using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using GeekathonAutoSync.BackUpLogs;
using GeekathonAutoSync.CloudStorages;
using GeekathonAutoSync.MultiTenancy;
using GeekathonAutoSync.SourceConfiguations;
using GeekathonAutoSync.StorageMasterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekathonAutoSync.BackUpStorageConfiguations
{
    public class BackUpStorageConfiguation : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public Guid StorageMasterTypeId { get; set; }
        [ForeignKey("StorageMasterTypeId")]
        public virtual StorageMasterType StorageMasterType { get; set; }
        public Guid? CloudStorageId { get; set; }
        [ForeignKey("CloudStorageId")]
        public virtual CloudStorage CloudStorage { get; set; }
        public string NFS_IP { get; set; }
        public string NFS_AccessUserID { get; set; }
        public string NFS_Password { get; set; }
        public string NFS_LocationPath { get; set; }
        public string AWS_AccessKey { get; set; }
        public string AWS_SecretKey { get; set; }
        public string AWS_BucketName { get; set; }
        public string AWS_Region { get; set; }
        public string AWS_backUpPath { get; set; }
        public string AZ_AccountName { get; set; }
        public string AZ_AccountKey { get; set; }
        public ICollection<SourceConfiguation> SourceConfiguations { get; set; } = new List<SourceConfiguation>();
        public ICollection<BackUpLog> BackUpLogs { get; set; } = new List<BackUpLog>();
    }
}
