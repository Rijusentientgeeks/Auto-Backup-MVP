using Abp.Domain.Entities.Auditing;
using GeekathonAutoSync.BackUpFrequencys.Dto;
using GeekathonAutoSync.SourceConfiguations.Dto;
using System;

namespace GeekathonAutoSync.BackUpSchedules.Dto
{
    public class BackUpScheduleDto : FullAuditedEntity<Guid>
    {
        public int TenantId { get; set; }
        public Guid? SourceConfiguationId { get; set; }
        public DateTime? BackupDate { get; set; }
        public TimeSpan? BackupTime { get; set; }
        public Guid? BackUpFrequencyId { get; set; }
        public string CronExpression { get; set; }
        public bool IsRemoveFromHangfire { get; set; }
        public SourceConfiguationDto SourceConfiguation { get; set; } = new SourceConfiguationDto();
        public BackUpFrequencyDto BackUpFrequency { get; set; } = new BackUpFrequencyDto();
    }
}
