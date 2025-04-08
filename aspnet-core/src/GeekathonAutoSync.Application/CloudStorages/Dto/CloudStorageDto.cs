using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.CloudStorages.Dto
{
    public class CloudStorageDto : FullAuditedEntity<Guid>
    {
        public string Name { get; set; }
        public CloudStorageType? Type { get; set; }
    }
}
