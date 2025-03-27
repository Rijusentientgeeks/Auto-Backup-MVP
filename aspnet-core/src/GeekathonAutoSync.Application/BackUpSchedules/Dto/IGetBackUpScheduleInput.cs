using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpSchedules.Dto
{
    public interface IGetBackUpScheduleInput : ISortedResultRequest
    {
        string FilterText { get; set; }
    }
}
