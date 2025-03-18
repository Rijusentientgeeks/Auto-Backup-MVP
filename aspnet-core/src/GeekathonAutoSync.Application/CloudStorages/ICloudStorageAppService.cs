using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeekathonAutoSync.CloudStorages.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.CloudStorages
{
    public interface ICloudStorageAppService : IApplicationService
    {
        Task<PagedResultDto<CloudStorageDto>> GetAllAsync();
    }
}
