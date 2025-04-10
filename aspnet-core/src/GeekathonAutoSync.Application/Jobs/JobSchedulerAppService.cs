using Abp.Domain.Uow;
using Abp.UI;
using GeekathonAutoSync.AutoBackup;
using GeekathonAutoSync.BackUpSchedules.Dto;
using Hangfire;
using System;
using System.Threading.Tasks;

namespace GeekathonAutoSync.Jobs
{
    public class JobSchedulerAppService : GeekathonAutoSyncAppServiceBase, IJobSchedulerAppService
    {
        private readonly IAutoBackupAppService _autoBackupAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public JobSchedulerAppService(IAutoBackupAppService autoBackupAppService, IUnitOfWorkManager unitOfWorkManager)
        {
            _autoBackupAppService = autoBackupAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async void ScheduleJobs(BackUpScheduleDto backUpSchedule)
        {
            RecurringJob.AddOrUpdate(
                $"Tenant-{backUpSchedule.TenantId} , Job-{backUpSchedule.Id}",  // BackUpName
                () => ExecuteBackupJob(backUpSchedule.TenantId, backUpSchedule.SourceConfiguationId.Value),
                backUpSchedule.CronExpression
            );
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ExecuteBackupJob(int tenantId, Guid sourceConfigurationId)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _autoBackupAppService.CreateBackup(sourceConfigurationId.ToString());
                    await uow.CompleteAsync();
                }
            }

        }


        public async void RemoveScheduleJobs(int tenantId, Guid backUpScheduleId)
        {
            //Remove the schedule if it exists
            RecurringJob.RemoveIfExists(
                $"Tenant-{tenantId} , Job-{backUpScheduleId}"
            );
        }
    }
}
