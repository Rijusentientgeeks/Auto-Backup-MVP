using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpSchedules.Dto
{
    public class BackUpScheduleUpdateDto : BackUpScheduleCreateDto
    {
        public Guid Id { get; set; }
    }
}
