using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using GeekathonAutoSync.EntityFrameworkCore.Seed;

namespace GeekathonAutoSync.EntityFrameworkCore
{
    [DependsOn(
        typeof(GeekathonAutoSyncCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class GeekathonAutoSyncEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<GeekathonAutoSyncDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        GeekathonAutoSyncDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        GeekathonAutoSyncDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(GeekathonAutoSyncEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
