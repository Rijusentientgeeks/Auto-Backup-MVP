using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using GeekathonAutoSync.BackUpStorageConfiguations.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.UI;
using GeekathonAutoSync.CloudStorages;
using GeekathonAutoSync.StorageMasterTypes;
using Abp.Authorization;
using GeekathonAutoSync.Authorization;

namespace GeekathonAutoSync.BackUpStorageConfiguations
{
    [AbpAuthorize(PermissionNames.Pages_BackupStorageConfiguration)]
    public class BackUpStorageConfiguationAppService : GeekathonAutoSyncAppServiceBase, IBackUpStorageConfiguationAppService
    {
        private readonly IRepository<BackUpStorageConfiguation, Guid> _backUpStorageConfiguationRepository;
        private readonly IRepository<StorageMasterType, Guid> _storageMasterTypeRepository;
        private readonly IRepository<CloudStorage, Guid> _cloudStorageRepository;
        public BackUpStorageConfiguationAppService(
            IRepository<BackUpStorageConfiguation, Guid> backUpStorageConfiguationRepository,
            IRepository<StorageMasterType, Guid> storageMasterTypeRepository,
            IRepository<CloudStorage, Guid> cloudStorageRepository)
        {
            _backUpStorageConfiguationRepository = backUpStorageConfiguationRepository;
            _storageMasterTypeRepository = storageMasterTypeRepository;
            _cloudStorageRepository = cloudStorageRepository;
        }

        public async Task<PagedResultDto<BackUpStorageConfiguationDto>> GetAllAsync(GetBackUpStorageConfiguationInput input)
        {
            var query = GetDetails(input);
            var backUpStorageConfiguationList = ObjectMapper.Map<List<BackUpStorageConfiguationDto>>(query);
            var pagedBackUpStorageConfiguations = backUpStorageConfiguationList
                    .AsQueryable()
                    .OrderBy(input.Sorting)
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();
            var backUpStorageConfiguationCount = query.Count();
            return new PagedResultDto<BackUpStorageConfiguationDto>
            {
                TotalCount = backUpStorageConfiguationCount,
                Items = pagedBackUpStorageConfiguations
            };
        }
        private IQueryable<BackUpStorageConfiguation> GetDetails(IGetBackUpStorageConfiguationInput input)
        {
            var query = _backUpStorageConfiguationRepository.GetAll()
                .Include(i => i.CloudStorage)
                .Include(i => i.StorageMasterType)
                .Include(i => i.SourceConfiguations)
                .Include(i => i.BackUpLogs)
                .Where(i => i.TenantId == AbpSession.TenantId);
                //.WhereIf(!input.FilterText.IsNullOrWhiteSpace(),
                //u => u.PlatformOrderId.Contains(input.FilterText)).AsQueryable()
            return query;
        }
        public async Task<BackUpStorageConfiguationDto> CreateAsync(BackUpStorageConfiguationCreateDto input)  
        {
            Guid backUpStorageID = Guid.Empty;
            await CheckValidation(input.StorageMasterTypeId, input.CloudStorageId);
            BackUpStorageConfiguation backUpStorageConfiguation = new BackUpStorageConfiguation
            {
                TenantId = (int)AbpSession.TenantId,
                StorageMasterTypeId = input.StorageMasterTypeId,
                CloudStorageId = input.CloudStorageId,
                NFS_IP = input.NFS_IP,
                NFS_AccessUserID = input.NFS_AccessUserID,
                NFS_Password = input.NFS_Password,
                NFS_LocationPath = input.NFS_LocationPath,
                AWS_AccessKey = input.AWS_AccessKey,
                AWS_SecretKey = input.AWS_SecretKey,
                AWS_BucketName  = input.AWS_BucketName,
                AWS_Region = input.AWS_Region,
                AWS_backUpPath = input.AWS_backUpPath,
                AZ_AccountName = input.AZ_AccountName,
                AZ_AccountKey = input.AZ_AccountKey,
            };
            using (CurrentUnitOfWork.SetTenantId(backUpStorageConfiguation.TenantId))
            {
                backUpStorageID = await _backUpStorageConfiguationRepository.InsertAndGetIdAsync(backUpStorageConfiguation);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            var getBackupStorageConfiguration = await GetAsync(backUpStorageID);
            return getBackupStorageConfiguration;
        }
        public async Task<BackUpStorageConfiguationDto> UpdateAsync(BackUpStorageConfiguationUpdateDto input)
        {
            var getBackupStorageConfiguration = await _backUpStorageConfiguationRepository.FirstOrDefaultAsync(i => i.Id == input.Id);
            if (getBackupStorageConfiguration == null)
            {
                throw new UserFriendlyException("Invalid backup storage configuration id");
            }
            Guid backUpStorageID = getBackupStorageConfiguration.Id;
            await CheckValidation(input.StorageMasterTypeId, input.CloudStorageId);
            getBackupStorageConfiguration.StorageMasterTypeId = input.StorageMasterTypeId;
            getBackupStorageConfiguration.CloudStorageId = input.CloudStorageId;
            getBackupStorageConfiguration.NFS_IP = input.NFS_IP;
            getBackupStorageConfiguration.NFS_AccessUserID = input.NFS_AccessUserID;
            getBackupStorageConfiguration.NFS_Password = input.NFS_Password;
            getBackupStorageConfiguration.NFS_LocationPath = input.NFS_LocationPath;
            getBackupStorageConfiguration.AWS_AccessKey = input.AWS_AccessKey;
            getBackupStorageConfiguration.AWS_SecretKey = input.AWS_SecretKey;
            getBackupStorageConfiguration.AWS_BucketName = input.AWS_BucketName;
            getBackupStorageConfiguration.AWS_Region = input.AWS_Region;
            getBackupStorageConfiguration.AWS_backUpPath = input.AWS_backUpPath;
            getBackupStorageConfiguration.AZ_AccountName = input.AZ_AccountName;
            getBackupStorageConfiguration.AZ_AccountKey = input.AZ_AccountKey;
            using (CurrentUnitOfWork.SetTenantId(getBackupStorageConfiguration.TenantId))
            {
                backUpStorageID = await _backUpStorageConfiguationRepository.InsertOrUpdateAndGetIdAsync(getBackupStorageConfiguration);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            var getBackupStorageConfigurationAfterUpdate = await GetAsync(backUpStorageID);
            return getBackupStorageConfigurationAfterUpdate;
        }
        public async Task<BackUpStorageConfiguationDto> GetAsync(Guid id)
        {
            var query = await _backUpStorageConfiguationRepository.GetAll()
                .Include(i => i.CloudStorage)
                .Include(i => i.StorageMasterType)
                .FirstOrDefaultAsync(i => i.Id == id);
            var backUpStorageConfiguation = ObjectMapper.Map<BackUpStorageConfiguationDto>(query);
            return backUpStorageConfiguation;
        }
        private async Task CheckValidation(Guid? storageMasterTypeId, Guid? cloudStorageId)
        {
            if (storageMasterTypeId == Guid.Empty || storageMasterTypeId == null)
            {
                throw new UserFriendlyException("Please enter storage type.");
            }
            else
            {
                var getstorageType = await _storageMasterTypeRepository.FirstOrDefaultAsync(i => i.Id == storageMasterTypeId);
                if (getstorageType == null)
                {
                    throw new UserFriendlyException("Please enter valid storage type.");
                }
            }
            if (cloudStorageId != Guid.Empty)
            {
                var getCloudStorage = await _cloudStorageRepository.FirstOrDefaultAsync(i => i.Id == cloudStorageId);
                if (getCloudStorage == null)
                {
                    throw new UserFriendlyException("Please enter valid cloud storage.");
                }
            }
        }
    }
}
