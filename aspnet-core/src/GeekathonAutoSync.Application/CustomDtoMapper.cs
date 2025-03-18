using AutoMapper;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUpStorageConfiguations.Dto;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.BackUPTypes.Dto;
using GeekathonAutoSync.CloudStorages;
using GeekathonAutoSync.CloudStorages.Dto;
using GeekathonAutoSync.DBTypes;
using GeekathonAutoSync.DBTypes.Dto;
using GeekathonAutoSync.StorageMasterTypes;
using GeekathonAutoSync.StorageMasterTypes.Dto;

namespace GeekathonAutoSync
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<StorageMasterType, StorageMasterTypeDto>();
            configuration.CreateMap<BackUPType, BackUPTypeDto>();
            configuration.CreateMap<CloudStorage, CloudStorageDto>();
            configuration.CreateMap<DBType, DBTypeDto>();
            configuration.CreateMap<StorageMasterType, StorageMasterTypeDto>();
            configuration.CreateMap<BackUpStorageConfiguation, BackUpStorageConfiguationDto>();
        }
    }
}
