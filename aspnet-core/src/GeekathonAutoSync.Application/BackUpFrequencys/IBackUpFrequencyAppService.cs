using Abp.Application.Services.Dto;
using Abp.Application.Services;
using System.Threading.Tasks;
using GeekathonAutoSync.BackUpFrequencys.Dto;

namespace GeekathonAutoSync.BackUpFrequencys
{
    public interface IBackUpFrequencyAppService : IApplicationService
    {
        Task<PagedResultDto<BackUpFrequencyDto>> GetAllAsync();
    }
}
