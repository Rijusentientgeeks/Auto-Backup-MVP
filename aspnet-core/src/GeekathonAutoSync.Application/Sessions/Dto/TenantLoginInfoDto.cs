using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using GeekathonAutoSync.MultiTenancy;

namespace GeekathonAutoSync.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
