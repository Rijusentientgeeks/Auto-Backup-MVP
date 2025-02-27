using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using GeekathonAutoSync.EntityFrameworkCore;
using GeekathonAutoSync.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace GeekathonAutoSync.Web.Tests
{
    [DependsOn(
        typeof(GeekathonAutoSyncWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class GeekathonAutoSyncWebTestModule : AbpModule
    {
        public GeekathonAutoSyncWebTestModule(GeekathonAutoSyncEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(GeekathonAutoSyncWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(GeekathonAutoSyncWebMvcModule).Assembly);
        }
    }
}