using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using GeekathonAutoSync.CloudStorages.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekathonAutoSync.CloudStorages
{
    public class CloudStorageAppService : GeekathonAutoSyncAppServiceBase, ICloudStorageAppService
    {
        private readonly IRepository<CloudStorage, Guid> _cloudStorageRepository;
        public CloudStorageAppService(IRepository<CloudStorage, Guid> cloudStorageRepository)
        {
            _cloudStorageRepository = cloudStorageRepository;
        }

        public async Task<PagedResultDto<CloudStorageDto>> GetAllAsync()
        {
            var query = GetDetails();
            var cloudStorageList = ObjectMapper.Map<List<CloudStorageDto>>(query);
            var pagedCloudStorages = cloudStorageList
                    .AsQueryable()
                    .OrderBy(i => i.Name)
                    .ToList();
            var cloudStorageCount = query.Count();
            return new PagedResultDto<CloudStorageDto>
            {
                TotalCount = cloudStorageCount,
                Items = pagedCloudStorages
            };
        }
        private IQueryable<CloudStorage> GetDetails()
        {
            var query = _cloudStorageRepository.GetAll();
            return query;
        }
    }
}
