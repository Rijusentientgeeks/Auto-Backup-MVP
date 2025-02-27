using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using GeekathonAutoSync.Configuration;
using GeekathonAutoSync.EntityFrameworkCore;
using GeekathonAutoSync.Migrator.DependencyInjection;

namespace GeekathonAutoSync.Migrator
{
    [DependsOn(typeof(GeekathonAutoSyncEntityFrameworkModule))]
    public class GeekathonAutoSyncMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public GeekathonAutoSyncMigratorModule(GeekathonAutoSyncEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(GeekathonAutoSyncMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                GeekathonAutoSyncConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(GeekathonAutoSyncMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
