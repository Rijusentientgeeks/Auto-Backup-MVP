using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeekathonAutoSync.SourceConfiguations.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SourceConfiguations
{
    public interface ISourceConfiguationAppService : IApplicationService
    {
        Task<PagedResultDto<SourceConfiguationDto>> GetAllAsync(GetSourceConfiguationInput input);
        Task<SourceConfiguationDto> GetAsync(Guid id);
        Task<SourceConfiguationDto> CreateAsync(SourceConfiguationCreateDto input);
        Task<SourceConfiguationDto> UpdateAsync(SourceConfiguationUpdateDto input);
    }
}
