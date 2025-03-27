using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.DBTypes.Dto
{
    public class DBTypeDto : FullAuditedEntity<Guid>
    {
        public string Name { get; set; }
    }
}
