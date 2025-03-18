using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using GeekathonAutoSync.StorageMasterTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekathonAutoSync.StorageMasterTypes
{
    public class StorageMasterTypeAppService : GeekathonAutoSyncAppServiceBase, IStorageMasterTypeAppService
    {
        private readonly IRepository<StorageMasterType, Guid> _storageMasterTypeRepository;
        public StorageMasterTypeAppService(IRepository<StorageMasterType, Guid> storageMasterTypeRepository)
        {
            _storageMasterTypeRepository = storageMasterTypeRepository;
        }
        public async Task<PagedResultDto<StorageMasterTypeDto>> GetAllAsync()
        {
            var query = GetDetails();
            var proofingRequestList = ObjectMapper.Map<List<StorageMasterTypeDto>>(query);
            var pagedProofingRequests = proofingRequestList
                    .AsQueryable()
                    .OrderBy(i => i.Name)
                    .ToList();
            var proofingRequestCount = query.Count();
            return new PagedResultDto<StorageMasterTypeDto>
            {
                TotalCount = proofingRequestCount,
                Items = pagedProofingRequests
            };
        }
        private IQueryable<StorageMasterType> GetDetails()
        {
            var query = _storageMasterTypeRepository.GetAll();
            return query;
        }
    }
}
