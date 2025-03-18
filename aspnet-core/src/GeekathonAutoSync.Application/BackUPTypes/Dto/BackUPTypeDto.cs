using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUPTypes.Dto
{
    public class BackUPTypeDto : FullAuditedEntity<Guid>
    {
        public string Name { get; set; }
    }
}
