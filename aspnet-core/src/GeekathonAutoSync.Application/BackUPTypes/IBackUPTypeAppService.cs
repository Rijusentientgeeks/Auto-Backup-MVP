using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeekathonAutoSync.BackUPTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUPTypes
{
    public interface IBackUPTypeAppService : IApplicationService
    {
        Task<PagedResultDto<BackUPTypeDto>> GetAllAsync();
    }
}
