using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using GeekathonAutoSync.Authorization;

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
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(GeekathonAutoSyncApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
