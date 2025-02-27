using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace GeekathonAutoSync.Controllers
{
    public abstract class GeekathonAutoSyncControllerBase: AbpController
    {
        protected GeekathonAutoSyncControllerBase()
        {
            LocalizationSourceName = GeekathonAutoSyncConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
