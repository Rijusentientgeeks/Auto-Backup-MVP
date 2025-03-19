using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SourceConfiguations.Dto
{
    public class SourceConfiguationUpdateDto : SourceConfiguationCreateDto
    {
        public Guid Id { get; set; }
    }
}
