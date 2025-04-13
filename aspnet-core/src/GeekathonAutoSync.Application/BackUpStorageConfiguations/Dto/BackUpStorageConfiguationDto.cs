using Abp.Domain.Entities.Auditing;
using GeekathonAutoSync.CloudStorages.Dto;
using GeekathonAutoSync.SourceConfiguations;
using GeekathonAutoSync.SourceConfiguations.Dto;
using GeekathonAutoSync.StorageMasterTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpStorageConfiguations.Dto
{
    public class BackUpStorageConfiguationDto : FullAuditedEntity<Guid>
    {
        public int TenantId { get; set; }
        public Guid StorageMasterTypeId { get; set; }
        public Guid? CloudStorageId { get; set; }
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
        public string BackupName { get; set; }
        public StorageMasterTypeDto StorageMasterType { get; set; } = new StorageMasterTypeDto();
        public CloudStorageDto CloudStorage { get; set; } = new CloudStorageDto();
        public bool IsUserLocalSystem { get; set; }
        public string Endpoint { get; set; }
        public string ProjectID { get; set; }
        public string CredentialFile { get; set; }
        //public List<SourceConfiguationDto> SourceConfiguations { get; set; } = new List<SourceConfiguationDto>();
        //public List<BackUpLogDto> BackUpLogs { get; set; } = new List<BackUpLogDto>();
    }
}
