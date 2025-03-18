using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeekathonAutoSync.BackUpStorageConfiguations.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpStorageConfiguations
{
    public interface IBackUpStorageConfiguationAppService : IApplicationService
    {
        Task<PagedResultDto<BackUpStorageConfiguationDto>> GetAllAsync(GetBackUpStorageConfiguationInput input);
        Task<BackUpStorageConfiguationDto> CreateAsync(BackUpStorageConfiguationCreateDto input);
        Task<BackUpStorageConfiguationDto> UpdateAsync(BackUpStorageConfiguationUpdateDto input);
        Task<BackUpStorageConfiguationDto> GetAsync(Guid id);
    }
}
