using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeekathonAutoSync.StorageMasterTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.StorageMasterTypes
{
    public interface IStorageMasterTypeAppService : IApplicationService
    {
        Task<PagedResultDto<StorageMasterTypeDto>> GetAllAsync();
    }
}
