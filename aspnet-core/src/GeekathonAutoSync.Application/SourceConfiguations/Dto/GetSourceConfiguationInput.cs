using Abp.Runtime.Validation;
using GeekathonAutoSync.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SourceConfiguations.Dto
{
    public class GetSourceConfiguationInput : PagedAndSortedInputDto, IShouldNormalize, IGetSourceConfiguationInput
    {
        public string FilterText { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }

            FilterText = FilterText?.Trim();
        }
    }
}
