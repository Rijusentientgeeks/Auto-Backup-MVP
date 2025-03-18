using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpStorageConfiguations.Dto
{
    public class BackUpStorageConfiguationUpdateDto : BackUpStorageConfiguationCreateDto
    {
        public Guid Id { get; set; }
    }
}
