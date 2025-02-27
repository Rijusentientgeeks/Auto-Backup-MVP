using System.Threading.Tasks;
using Abp.Application.Services;
using GeekathonAutoSync.Sessions.Dto;

namespace GeekathonAutoSync.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
