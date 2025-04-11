using Abp.Application.Services;
using GeekathonAutoSync.SpeechRecognitions.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SpeechRecognitions
{
    public interface ISpeechRecognitionAppService : IApplicationService
    {
        Task<bool> ReceiveCommandAsync([FromBody] CommandDto model);
    }
}
