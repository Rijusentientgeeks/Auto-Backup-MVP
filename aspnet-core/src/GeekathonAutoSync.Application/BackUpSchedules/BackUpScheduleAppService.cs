using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using GeekathonAutoSync.BackUpFrequencys;
using GeekathonAutoSync.BackUpSchedules.Dto;
using GeekathonAutoSync.SourceConfiguations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using GeekathonAutoSync.Jobs;
using Microsoft.Extensions.Options;
using CronExpressionDescriptor;
using Options = CronExpressionDescriptor.Options;

namespace GeekathonAutoSync.BackUpSchedules
{
    public class BackUpScheduleAppService : GeekathonAutoSyncAppServiceBase, IBackUpScheduleAppService
    {
        private readonly IRepository<BackUpSchedule, Guid> _backUpScheduleRepository;
        private readonly IRepository<SourceConfiguation, Guid> _sourceConfiguationRepository;
        private readonly IRepository<BackUpFrequency, Guid> _backUpFrequencyRepository;
        private readonly IJobSchedulerAppService _scheduler;

        public BackUpScheduleAppService(
            IRepository<BackUpSchedule, Guid> backUpScheduleRepository,
            IRepository<SourceConfiguation, Guid> sourceConfiguationRepository,
            IRepository<BackUpFrequency, Guid> backUpFrequencyRepository,
            IJobSchedulerAppService scheduler
            )
        {
            _backUpScheduleRepository = backUpScheduleRepository;
            _sourceConfiguationRepository = sourceConfiguationRepository;
            _backUpFrequencyRepository = backUpFrequencyRepository;
            _scheduler = scheduler;
        }
        public async Task<PagedResultDto<BackUpScheduleDto>> GetAllAsync(GetBackUpScheduleInput input)
        {
            var query = GetDetails(input);
            var backUpScheduleList = ObjectMapper.Map<List<BackUpScheduleDto>>(query);
            var pagedBackUpSchedules = backUpScheduleList
                    .AsQueryable()
                    .OrderBy(input.Sorting)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            var backUpScheduleCount = query.Count();
            foreach (var item in pagedBackUpSchedules)
            {
                item.CronExpression = await ConvertCronExpToRegularValue(item.CronExpression);
            }
            return new PagedResultDto<BackUpScheduleDto>
            {
                TotalCount = backUpScheduleCount,
                Items = pagedBackUpSchedules
            };
        }
        private IQueryable<BackUpSchedule> GetDetails(IGetBackUpScheduleInput input)
        {
            var query = _backUpScheduleRepository.GetAll()
                .Include(i => i.BackUpFrequency)
                .Include(i => i.SourceConfiguation)
                .Where(i => i.TenantId == AbpSession.TenantId);
            //.WhereIf(!input.FilterText.IsNullOrWhiteSpace(),
            //u => u.PlatformOrderId.Contains(input.FilterText)).AsQueryable()
            return query;
        }
        public async Task<BackUpScheduleDto> CreateAsync(BackUpScheduleCreateDto input)
        {
            Guid backUpScheduleID = Guid.Empty;
            await CheckValidation(input.SourceConfiguationId, input.BackUpFrequencyId);
            BackUpSchedule backUpSchedule = new BackUpSchedule
            {
                TenantId = (int)AbpSession.TenantId,
                SourceConfiguationId = input.SourceConfiguationId,
                BackupDate = input.BackupDate,
                BackupTime = input.BackupTime,
                BackUpFrequencyId = input.BackUpFrequencyId,
                CronExpression = input.CronExpression,
                IsRemoveFromHangfire = false,
            };
            using (CurrentUnitOfWork.SetTenantId(backUpSchedule.TenantId))
            {
                backUpScheduleID = await _backUpScheduleRepository.InsertAndGetIdAsync(backUpSchedule);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            var getBackUpSchedule = await GetAsync(backUpScheduleID);
            _scheduler.ScheduleJobs(getBackUpSchedule);
            return getBackUpSchedule;
        }
        public async Task<BackUpScheduleDto> UpdateAsync(BackUpScheduleUpdateDto input)
        {
            if (input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("Invalid backup schedule.");
            }
            var getBackUpSchedule = await _backUpScheduleRepository.FirstOrDefaultAsync(i => i.Id == input.Id);
            if (getBackUpSchedule == null)
            {
                throw new UserFriendlyException("Invalid backup schedule id");
            }
            Guid backUpScheduleID = getBackUpSchedule.Id;
            await CheckValidation(input.SourceConfiguationId, input.BackUpFrequencyId);
            getBackUpSchedule.SourceConfiguationId = input.SourceConfiguationId;
            getBackUpSchedule.BackUpFrequencyId = input.BackUpFrequencyId;
            getBackUpSchedule.BackupDate = input.BackupDate;
            getBackUpSchedule.BackupTime = input.BackupTime;
            getBackUpSchedule.CronExpression = input.CronExpression;
            using (CurrentUnitOfWork.SetTenantId(getBackUpSchedule.TenantId))
            {
                backUpScheduleID = await _backUpScheduleRepository.InsertOrUpdateAndGetIdAsync(getBackUpSchedule);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            var getBackUpScheduleAfterUpdate = await GetAsync(backUpScheduleID);
            _scheduler.ScheduleJobs(getBackUpScheduleAfterUpdate);
            return getBackUpScheduleAfterUpdate;
        }
        public async Task<BackUpScheduleDto> GetAsync(Guid id)
        {
            var query = await _backUpScheduleRepository.GetAll()
                .Include(i => i.BackUpFrequency)
                .Include(i => i.SourceConfiguation)
                .FirstOrDefaultAsync(i => i.Id == id);
            var backUpSchedule = ObjectMapper.Map<BackUpScheduleDto>(query);
            return backUpSchedule;
        }
        public async Task<bool> RemoveScheduleAsync(Guid backupScheduleId)
        {
            bool IsRemove = false;
            if (backupScheduleId == Guid.Empty)
            {
                throw new UserFriendlyException("Invalid backup schedule.");
            }
            var getBackUpSchedule = await _backUpScheduleRepository.FirstOrDefaultAsync(i => i.Id == backupScheduleId);
            if (getBackUpSchedule == null) 
            {
                throw new UserFriendlyException("Invalid backup schedule.");
            }
            if (getBackUpSchedule.IsRemoveFromHangfire == true)
            {
                throw new UserFriendlyException("Already remove this schedule.");
            }
            _scheduler.RemoveScheduleJobs(getBackUpSchedule.TenantId, getBackUpSchedule.Id);
            getBackUpSchedule.IsRemoveFromHangfire = true;
            IsRemove = true;
            return IsRemove;
        }
        private async Task CheckValidation(Guid? sourceConfiguationId, Guid? backUpFrequencyId)
        {
            if (sourceConfiguationId == Guid.Empty || sourceConfiguationId == null)
            {
                throw new UserFriendlyException("Please choice source configuation.");
            }
            else
            {
                var getsourceConfiguation = await _sourceConfiguationRepository.FirstOrDefaultAsync(i => i.Id == sourceConfiguationId);
                if (getsourceConfiguation == null)
                {
                    throw new UserFriendlyException("Please choice valid source configuation.");
                }
            }
            if (backUpFrequencyId == Guid.Empty || backUpFrequencyId == null)
            {
                throw new UserFriendlyException("Please choice backUp frequency.");
            }
            else
            {
                var getbackUpFrequency = await _backUpFrequencyRepository.FirstOrDefaultAsync(i => i.Id == backUpFrequencyId);
                if (getbackUpFrequency == null)
                {
                    throw new UserFriendlyException("Please choice valid backUp frequency.");
                }
            }
        }
        private async Task<string> ConvertCronExpToRegularValue(string cronExp)
        {
            var options = new Options
            {
                Locale = "en", // or "fr", "de", etc.
                Use24HourTimeFormat = false
            };
            var description = ExpressionDescriptor.GetDescription(cronExp, options);

            return description;
        }
    }
}
