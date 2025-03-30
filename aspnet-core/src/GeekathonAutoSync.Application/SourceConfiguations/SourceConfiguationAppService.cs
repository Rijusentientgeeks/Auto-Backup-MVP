using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using GeekathonAutoSync.Authorization;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.DBTypes;
using GeekathonAutoSync.SourceConfiguations.Dto;
using GeekathonAutoSync.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SourceConfiguations
{
    [AbpAuthorize(PermissionNames.Pages_SourceConfiguation)]
    public class SourceConfiguationAppService : GeekathonAutoSyncAppServiceBase, ISourceConfiguationAppService
    {
        private readonly IRepository<SourceConfiguation, Guid> _sourceConfiguationRepository;
        private readonly IRepository<BackUPType, Guid> _backUPTypeRepository;
        private readonly IRepository<DBType, Guid> _dBTypeRepository;
        private readonly IRepository<BackUpStorageConfiguation, Guid> _backUpStorageConfiguationRepository;
        public SourceConfiguationAppService(
            IRepository<SourceConfiguation, Guid> sourceConfiguationRepository,
            IRepository<BackUPType, Guid> backUPTypeRepository,
            IRepository<DBType, Guid> dBTypeRepository,
            IRepository<BackUpStorageConfiguation, Guid> backUpStorageConfiguationRepository)
        {
            _sourceConfiguationRepository = sourceConfiguationRepository;
            _backUPTypeRepository = backUPTypeRepository;
            _dBTypeRepository = dBTypeRepository;
            _backUpStorageConfiguationRepository = backUpStorageConfiguationRepository;
        }
        public async Task<PagedResultDto<SourceConfiguationDto>> GetAllAsync(GetSourceConfiguationInput input)
        {
            var query = GetDetails(input);
            var sourceConfiguationList = ObjectMapper.Map<List<SourceConfiguationDto>>(query);
            var pagedSourceConfiguations = sourceConfiguationList
                    .AsQueryable()
                    .OrderBy(input.Sorting)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            var sourceConfiguationCount = query.Count();
            return new PagedResultDto<SourceConfiguationDto>
            {
                TotalCount = sourceConfiguationCount,
                Items = pagedSourceConfiguations
            };
        }
        private IQueryable<SourceConfiguation> GetDetails(IGetSourceConfiguationInput input)
        {
            var query = _sourceConfiguationRepository.GetAll()
                .Include(i => i.BackUPType)
                .Include(i => i.DBType)
                .Include(i => i.BackUpStorageConfiguation)
                .Include(i => i.BackUpLogs)
                .Include(i => i.BackUpSchedules)
                .Where(i => i.TenantId == AbpSession.TenantId);
            //.WhereIf(!input.FilterText.IsNullOrWhiteSpace(),
            //u => u.PlatformOrderId.Contains(input.FilterText)).AsQueryable()
            return query;
        }
        public async Task<SourceConfiguationDto> GetAsync(Guid id)
        {
            var query = await _sourceConfiguationRepository.GetAll()
                .Include(i => i.BackUPType)
                .Include(i => i.DBType)
                .Include(i => i.BackUpStorageConfiguation)
                .FirstOrDefaultAsync(i => i.Id == id);
            var sourceConfiguation = ObjectMapper.Map<SourceConfiguationDto>(query);
            return sourceConfiguation;
        }
        public async Task<SourceConfiguationDto> CreateAsync(SourceConfiguationCreateDto input)
        {
            Guid sourceConfiguationID = Guid.Empty;
            await CheckValidation(input.BackUPTypeId, input.DBTypeId, input.BackUpStorageConfiguationId);
            
            Base64ToPemConverter converter = new Base64ToPemConverter();
            var filePath =input.PrivateKeyPath==null?null: await converter.ConvertBase64ToPemFile(input.PrivateKeyPath, (int)AbpSession.TenantId);
            SourceConfiguation sourceConfiguation = new SourceConfiguation
            {
                TenantId = (int)AbpSession.TenantId,
                BackUPTypeId = input.BackUPTypeId,
                DBTypeId = input.DBTypeId,
                DatabaseName = input.DatabaseName,
                DbUsername = input.DbUsername,
                DbPassword = input.DbPassword,
                Port = input.Port,
                SshUserName = input.SshUserName,
                SshPassword = input.SshPassword,
                ServerIP = input.ServerIP,
                DBInitialCatalog = input.DBInitialCatalog,
                UserID = input.UserID,
                Password = input.Password,
                PrivateKeyPath = filePath,
                BackUpInitiatedPath = input.BackUpInitiatedPath,
                Sourcepath = input.Sourcepath,
                OS = input.OS,
                BackUpStorageConfiguationId = input.BackUpStorageConfiguationId,
                BackupName = input.BackupName
            };
            using (CurrentUnitOfWork.SetTenantId(sourceConfiguation.TenantId))
            {
                sourceConfiguationID = await _sourceConfiguationRepository.InsertAndGetIdAsync(sourceConfiguation);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            var getSourceConfiguation = await GetAsync(sourceConfiguationID);
            return getSourceConfiguation;
        }
        public async Task<SourceConfiguationDto> UpdateAsync(SourceConfiguationUpdateDto input)
        {
            var getSourceConfiguation = await _sourceConfiguationRepository.FirstOrDefaultAsync(i => i.Id == input.Id);
            if (getSourceConfiguation == null)
            {
                throw new UserFriendlyException("Invalid source configuration id");
            }
            Guid sourceConfiguationID = getSourceConfiguation.Id;
            await CheckValidation(input.BackUPTypeId, input.DBTypeId, input.BackUpStorageConfiguationId);
            getSourceConfiguation.BackUPTypeId = input.BackUPTypeId;
            getSourceConfiguation.DBTypeId = input.DBTypeId;
            getSourceConfiguation.ServerIP = input.ServerIP;
            getSourceConfiguation.DatabaseName = input.DatabaseName;
            getSourceConfiguation.DbUsername = input.DbUsername;
            getSourceConfiguation.DbPassword = input.DbPassword;
            getSourceConfiguation.Port = input.Port;
            getSourceConfiguation.SshUserName = input.SshUserName;
            getSourceConfiguation.SshPassword = input.SshPassword;
            getSourceConfiguation.DBInitialCatalog = input.DBInitialCatalog;
            getSourceConfiguation.UserID = input.UserID;
            getSourceConfiguation.Password = input.Password;
            //getSourceConfiguation.PrivateKeyPath = input.PrivateKeyPath;
            getSourceConfiguation.BackUpInitiatedPath = input.BackUpInitiatedPath;
            getSourceConfiguation.Sourcepath = input.Sourcepath;
            getSourceConfiguation.OS = input.OS;
            getSourceConfiguation.BackUpStorageConfiguationId = input.BackUpStorageConfiguationId;
            getSourceConfiguation.BackupName = input.BackupName;
            using (CurrentUnitOfWork.SetTenantId(getSourceConfiguation.TenantId))
            {
                sourceConfiguationID = await _sourceConfiguationRepository.InsertOrUpdateAndGetIdAsync(getSourceConfiguation);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            var getSourceConfiguationAfterUpdate = await GetAsync(sourceConfiguationID);
            return getSourceConfiguationAfterUpdate;
        }
        private async Task CheckValidation(Guid? backUPTypeId, Guid? dBTypeId, Guid? backUpStorageConfiguationId)
        {
            if (backUPTypeId == Guid.Empty || backUPTypeId == null)
            {
                throw new UserFriendlyException("Please enter backup type.");
            }
            else
            {
                var getbackUPType = await _backUPTypeRepository.FirstOrDefaultAsync(i => i.Id == backUPTypeId);
                if (getbackUPType == null)
                {
                    throw new UserFriendlyException("Please enter valid backup type.");
                }
            }
            if (dBTypeId != Guid.Empty && dBTypeId != null)
            {
                var getDBTypeId = await _dBTypeRepository.FirstOrDefaultAsync(i => i.Id == dBTypeId);
                if (getDBTypeId == null)
                {
                    throw new UserFriendlyException("Please enter valid DB type.");
                }
            }
            if (backUpStorageConfiguationId != Guid.Empty && backUpStorageConfiguationId != null)
            {
                var getBackUpStorageConfiguationId = await _backUpStorageConfiguationRepository.FirstOrDefaultAsync(i => i.Id == backUpStorageConfiguationId);
                if (getBackUpStorageConfiguationId == null)
                {
                    throw new UserFriendlyException("Please enter valid backup storage configuation.");
                }
            }
        }
    }
}
