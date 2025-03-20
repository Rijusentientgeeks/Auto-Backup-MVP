using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpSchedules.Dto
{
    public class BackUpScheduleCreateDto
    {
        public Guid? SourceConfiguationId { get; set; }
        public DateTime? BackupDate { get; set; }
        public TimeSpan? BackupTime { get; set; }
        public Guid? BackUpFrequencyId { get; set; }
    }
}
