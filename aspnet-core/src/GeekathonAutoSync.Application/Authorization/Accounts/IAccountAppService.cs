using System.Threading.Tasks;
using Abp.Application.Services;
using GeekathonAutoSync.Authorization.Accounts.Dto;

namespace GeekathonAutoSync.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
