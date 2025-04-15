using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.StorageMasterTypes
{
    public enum StorageMasterTypeEnum
    {
        [Description("Public Cloud")]
        PublicCloud = 0,
        [Description("GeekSync Infrastructure Cluster")]
        GeekSyncInfrastructureCluste = 1,
        [Description("Network File System")]
        NFS = 2,
    }
}
