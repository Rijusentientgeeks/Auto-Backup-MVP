using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SourceConfiguations.Dto
{
    public interface IGetSourceConfiguationInput : ISortedResultRequest
    {
        string FilterText { get; set; }
    }
}
