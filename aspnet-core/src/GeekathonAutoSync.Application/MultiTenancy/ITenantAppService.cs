using Abp.Application.Services;
using GeekathonAutoSync.MultiTenancy.Dto;

namespace GeekathonAutoSync.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

