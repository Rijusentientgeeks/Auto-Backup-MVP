using Abp.Domain.Entities;
using GeekathonAutoSync.BackUpStorageConfiguations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.CloudStorages
{
    public class CloudStorage : Entity<Guid>
    {
        public CloudStorage(Guid id, string name, CloudStorageType? type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
        public string Name { get; set; }
        public CloudStorageType? Type { get; set; }
        ICollection<BackUpStorageConfiguation> BackUpStorageConfiguations { get; set; } = new List<BackUpStorageConfiguation>();
    }
}
