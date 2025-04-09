using Abp.Application.Services;
using GeekathonAutoSync.BackUpSchedules.Dto;
using System;

namespace GeekathonAutoSync.Jobs
{
    public interface IJobSchedulerAppService : IApplicationService
    {
        void ScheduleJobs(BackUpScheduleDto backUpSchedule);
        void RemoveScheduleJobs(int tenantId, Guid backUpFrequencyId);
    }
}
