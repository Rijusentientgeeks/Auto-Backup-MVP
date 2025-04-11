using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using GeekathonAutoSync.MultiTenancy;
using System.ComponentModel.DataAnnotations.Schema;
using GeekathonAutoSync.SourceConfiguations;
using GeekathonAutoSync.BackUpFrequencys;

namespace GeekathonAutoSync.BackUpSchedules
{
    public class BackUpSchedule : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public Guid? SourceConfiguationId { get; set; }
        [ForeignKey("SourceConfiguationId")]
        public virtual SourceConfiguation SourceConfiguation { get; set; }
        public DateTime? BackupDate { get; set; }
        public TimeSpan? BackupTime { get; set; }
        public string CronExpression { get; set; }
        public bool IsRemoveFromHangfire { get; set; }
        public Guid? BackUpFrequencyId { get; set; }
        [ForeignKey("BackUpFrequencyId")]
        public virtual BackUpFrequency BackUpFrequency { get; set; }
    }
}
