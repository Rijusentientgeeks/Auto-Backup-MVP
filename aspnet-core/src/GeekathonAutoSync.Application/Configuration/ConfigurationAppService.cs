using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using GeekathonAutoSync.Configuration.Dto;

namespace GeekathonAutoSync.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : GeekathonAutoSyncAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
