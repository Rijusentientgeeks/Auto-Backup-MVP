using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.BackUPTypes.Dto;
using GeekathonAutoSync.DBTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.DBTypes
{
    public class DBTypeAppService : GeekathonAutoSyncAppServiceBase, IDBTypeAppService
    {
        private readonly IRepository<DBType, Guid> _dBTypeRepository;
        public DBTypeAppService(IRepository<DBType, Guid> dBTypeRepository)
        {
            _dBTypeRepository = dBTypeRepository;
        }
        public async Task<PagedResultDto<DBTypeDto>> GetAllAsync()
        {
            var query = GetDetails();
            var dBTypeList = ObjectMapper.Map<List<DBTypeDto>>(query);
            var pagedDBTypes = dBTypeList
                    .AsQueryable()
                    .OrderBy(i => i.Name)
                    .ToList();
            var dBTypeCount = query.Count();
            return new PagedResultDto<DBTypeDto>
            {
                TotalCount = dBTypeCount,
                Items = pagedDBTypes
            };
        }
        private IQueryable<DBType> GetDetails()
        {
            var query = _dBTypeRepository.GetAll();
            return query;
        }
    }
}
