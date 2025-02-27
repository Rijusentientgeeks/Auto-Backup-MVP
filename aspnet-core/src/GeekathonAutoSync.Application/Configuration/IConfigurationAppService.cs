using System.Threading.Tasks;
using GeekathonAutoSync.Configuration.Dto;

namespace GeekathonAutoSync.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
