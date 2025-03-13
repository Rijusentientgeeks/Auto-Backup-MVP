using Abp.Domain.Entities;
using GeekathonAutoSync.SourceConfiguations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUPTypes
{
    public class BackUPType : Entity<Guid>
    {
        public BackUPType(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Name { get; set; }
        ICollection<SourceConfiguation> SourceConfiguations { get; set; } = new List<SourceConfiguation>();
    }
}
