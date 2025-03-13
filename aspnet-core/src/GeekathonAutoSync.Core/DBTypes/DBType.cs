using Abp.Domain.Entities;
using GeekathonAutoSync.SourceConfiguations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.DBTypes
{
    public class DBType : Entity<Guid>
    {
        public DBType(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Name { get; set; }
        ICollection<SourceConfiguation> SourceConfiguations { get; set; } = new List<SourceConfiguation>();
    }
}
