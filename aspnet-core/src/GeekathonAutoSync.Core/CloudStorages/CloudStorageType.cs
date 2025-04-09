using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.CloudStorages
{
    public enum CloudStorageType
    {
        [Description("Amazon S3")]
        AmazonS3 = 0,
        [Description("Microsoft Azure")]
        MicrosoftAzure = 1,
        [Description("Google Cloud")]
        GoogleCloud = 2,
        [Description("Alibaba Cloud")]
        AlibabaCloud = 3,
    }
}
