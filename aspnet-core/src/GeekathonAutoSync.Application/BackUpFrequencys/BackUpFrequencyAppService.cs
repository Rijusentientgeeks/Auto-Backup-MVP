using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using GeekathonAutoSync.BackUpFrequencys.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpFrequencys
{
    public class BackUpFrequencyAppService : GeekathonAutoSyncAppServiceBase, IBackUpFrequencyAppService
    {
        private readonly IRepository<BackUpFrequency, Guid> _backUpFrequencyRepository;
        public BackUpFrequencyAppService(IRepository<BackUpFrequency, Guid> backUpFrequencyRepository)
        {
            _backUpFrequencyRepository = backUpFrequencyRepository;
        }
        public async Task<PagedResultDto<BackUpFrequencyDto>> GetAllAsync()
        {
            var query = GetDetails();
            var backUpFrequencyList = ObjectMapper.Map<List<BackUpFrequencyDto>>(query);
            var pagedBackUpFrequencys = backUpFrequencyList
                    .AsQueryable()
                    .OrderBy(i => i.Name)
                    .ToList();
            var backUpFrequencyCount = query.Count();
            return new PagedResultDto<BackUpFrequencyDto>
            {
                TotalCount = backUpFrequencyCount,
                Items = pagedBackUpFrequencys
            };
        }
        private IQueryable<BackUpFrequency> GetDetails()
        {
            var query = _backUpFrequencyRepository.GetAll();
            return query;
        }
    }
}
