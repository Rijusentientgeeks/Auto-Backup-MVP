using Abp.Runtime.Validation;
using GeekathonAutoSync.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpStorageConfiguations.Dto
{
    public class GetBackUpStorageConfiguationInput : PagedAndSortedInputDto, IShouldNormalize, IGetBackUpStorageConfiguationInput
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
