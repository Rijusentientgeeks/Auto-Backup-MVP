using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using GeekathonAutoSync.Authorization;
using GeekathonAutoSync.Users.Dto;

namespace GeekathonAutoSync
{
    [DependsOn(
        typeof(GeekathonAutoSyncCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class GeekathonAutoSyncApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<GeekathonAutoSyncAuthorizationProvider>();
            
            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(GeekathonAutoSyncApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(typeof(GeekathonAutoSyncApplicationModule).GetAssembly());

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
