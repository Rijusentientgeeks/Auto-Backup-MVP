using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpStorageConfiguations.Dto
{
    public interface IGetBackUpStorageConfiguationInput : ISortedResultRequest
    {
        string FilterText { get; set; }
    }
}
