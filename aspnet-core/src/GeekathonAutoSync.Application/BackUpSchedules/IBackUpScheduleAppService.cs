using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeekathonAutoSync.BackUpSchedules.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpSchedules
{
    public interface IBackUpScheduleAppService : IApplicationService
    {
        Task<PagedResultDto<BackUpScheduleDto>> GetAllAsync(GetBackUpScheduleInput input);
        Task<BackUpScheduleDto> CreateAsync(BackUpScheduleCreateDto input);
        Task<BackUpScheduleDto> UpdateAsync(BackUpScheduleUpdateDto input);
        Task<BackUpScheduleDto> GetAsync(Guid id);
        Task<bool> RemoveScheduleAsync(Guid backupScheduleId);
    }
}
