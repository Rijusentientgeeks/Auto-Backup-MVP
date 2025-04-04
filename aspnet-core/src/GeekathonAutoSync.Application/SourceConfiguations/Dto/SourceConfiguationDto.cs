using Abp.Domain.Entities.Auditing;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUpStorageConfiguations.Dto;
using GeekathonAutoSync.BackUPTypes.Dto;
using GeekathonAutoSync.DBTypes.Dto;
using System;

namespace GeekathonAutoSync.SourceConfiguations.Dto
{
    public class SourceConfiguationDto : FullAuditedEntity<Guid>
    {
        public int TenantId { get; set; }
        public Guid BackUPTypeId { get; set; }
        public Guid? DBTypeId { get; set; }
        public string DatabaseName { get; set; }
        public string DbUsername { get; set; }
        public string DbPassword { get; set; }
        public string Port { get; set; }
        public string SshUserName { get; set; }
        public string SshPassword { get; set; }
        public string ServerIP { get; set; }
        public string DBInitialCatalog { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string PrivateKeyPath { get; set; }
        public string BackUpInitiatedPath { get; set; }
        public string Sourcepath { get; set; }
        public string OS { get; set; }
        public Guid? BackUpStorageConfiguationId { get; set; }
        public BackUPTypeDto BackUPType { get; set; } = new BackUPTypeDto();
        public DBTypeDto DBType { get; set; } = new DBTypeDto();
        public string BackupName { get; set; }
        public BackUpStorageConfiguationDto BackUpStorageConfiguation { get; set; } = new BackUpStorageConfiguationDto();
        //public List<BackUpScheduleDto> BackUpSchedules { get; set; } = new List<BackUpScheduleDto>();
        //public List<BackUpLogDto> BackUpLogs { get; set; } = new List<BackUpLogDto>();
    }
}
