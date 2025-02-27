using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using GeekathonAutoSync.Configuration;

namespace GeekathonAutoSync.Web.Host.Startup
{
    [DependsOn(
       typeof(GeekathonAutoSyncWebCoreModule))]
    public class GeekathonAutoSyncWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public GeekathonAutoSyncWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(GeekathonAutoSyncWebHostModule).GetAssembly());
        }
    }
}
