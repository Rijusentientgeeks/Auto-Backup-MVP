using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUPTypes
{
    public enum BackupTypeEnum
    {
        [Description("Database")]
        DataBase = 0,
        [Description("Application Files")]
        ApplicationFiles = 1,
        [Description("Specific File Backup")]
        SpecificFile = 2,
    }
}
