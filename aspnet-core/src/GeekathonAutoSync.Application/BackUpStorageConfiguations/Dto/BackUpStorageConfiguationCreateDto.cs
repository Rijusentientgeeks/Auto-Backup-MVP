using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.BackUpStorageConfiguations.Dto
{
    public class BackUpStorageConfiguationCreateDto
    {
        public Guid StorageMasterTypeId { get; set; }
        public Guid? CloudStorageId { get; set; }
        public string NFS_IP { get; set; }
        public string NFS_AccessUserID { get; set; }
        public string NFS_Password { get; set; }
        public string NFS_LocationPath { get; set; }
        public string AWS_AccessKey { get; set; }
        public string AWS_SecretKey { get; set; }
        public string AWS_BucketName { get; set; }
        public string AWS_Region { get; set; }
        public string AWS_backUpPath { get; set; }
        public string AZ_AccountName { get; set; }
        public string AZ_AccountKey { get; set; }
    }
}
