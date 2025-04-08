using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.DBTypes
{
    public enum DbTypeEnum
    {
        [Description("PostgreSQL")]
        PostgreSQL = 0,
        [Description("Microsoft SQL Server")]
        MicrosoftSQLServer = 1,
        [Description("Oracle Database")]
        OracleDatabase = 2,
        [Description("MySQL")]
        MySQL = 3,
        [Description("MongoDB")]
        MongoDB = 4,
    }
}
