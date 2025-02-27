using Abp.Application.Services.Dto;

namespace GeekathonAutoSync.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

