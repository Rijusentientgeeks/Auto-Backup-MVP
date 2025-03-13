using Abp.Domain.Entities;
using GeekathonAutoSync.BackUpSchedules;
using System;
using System.Collections.Generic;

namespace GeekathonAutoSync.BackUpFrequencys
{
    public class BackUpFrequency : Entity<Guid>
    {
        public BackUpFrequency(Guid id, string name) 
        {
            Id = id;
            Name = name;
        }
        public string Name { get; set; }
        ICollection<BackUpSchedule> BackUpSchedules { get; set; } = new List<BackUpSchedule>();
    }
}
