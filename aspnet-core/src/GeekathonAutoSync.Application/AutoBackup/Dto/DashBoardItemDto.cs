using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.AutoBackup.Dto
{
    public class DashBoardItemDto
    {
        public int BackupSourceCount { get; set; }
        public int BackupStorageCount { get; set; }
        public int ScheduleBackupCount { get; set; }
        public string LastBackupStatus { get; set; }
        public string LastBackupItem { get; set; }
        public int TotalBackupCount { get; set; }
        public List<Sconfig> NextScheduleList { get; set; } = new List<Sconfig>();
    }

    public class Sconfig
    {
        public string Name { get; set; }
        public string CronExo { get; set; }
    }
}
