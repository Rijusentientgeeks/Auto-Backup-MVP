using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekathonAutoSync.BackUPTypes.Dto;

namespace GeekathonAutoSync.BackUPTypes
{
    public class BackUPTypeAppService : GeekathonAutoSyncAppServiceBase, IBackUPTypeAppService
    {
        private readonly IRepository<BackUPType, Guid> _backUPTypeRepository;
        public BackUPTypeAppService(IRepository<BackUPType, Guid> backUPTypeRepository)
        {
            _backUPTypeRepository = backUPTypeRepository;
        }
        public async Task<PagedResultDto<BackUPTypeDto>> GetAllAsync()
        {
            var query = GetDetails();
            var backUPTypeList = ObjectMapper.Map<List<BackUPTypeDto>>(query);
            var pagedBackUPTypes = backUPTypeList
                    .AsQueryable()
                    .OrderBy(i => i.Name)
                    .ToList();
            var backUPTypeCount = query.Count();
            return new PagedResultDto<BackUPTypeDto>
            {
                TotalCount = backUPTypeCount,
                Items = pagedBackUPTypes
            };
        }
        private IQueryable<BackUPType> GetDetails()
        {
            var query = _backUPTypeRepository.GetAll();
            return query;
        }
    }
}
