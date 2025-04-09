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

        public JobSchedulerAppService(IAutoBackupAppService autoBackupAppService)
        {
            _autoBackupAppService = autoBackupAppService;
        }

        public async void ScheduleJobs(BackUpScheduleDto backUpSchedule)
        {
            //Schedule with Cron Expression
            RecurringJob.AddOrUpdate(
                $"Tenant-{backUpSchedule.TenantId} , Job-{backUpSchedule.Id}",  // BackUpName
                () => ExecuteBackupJob(backUpSchedule.TenantId, backUpSchedule.SourceConfiguationId.Value),
                backUpSchedule.CronExpression
            );
        }

        [AutomaticRetry(Attempts = 0)] // Retry on failure
        public async Task ExecuteBackupJob(int tenantId, Guid sourceConfigurationId)
        {
            try
            {
                var res = await _autoBackupAppService.CreateBackup(sourceConfigurationId.ToString());
                Console.WriteLine($"✅ Backup completed for Tenant: {tenantId}, {res}");
            }
            catch (Exception ex)
            {
                //throw new UserFriendlyException();
                Console.WriteLine($"Backup is not completed for Tenant: {tenantId}, {ex.Message}");
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
