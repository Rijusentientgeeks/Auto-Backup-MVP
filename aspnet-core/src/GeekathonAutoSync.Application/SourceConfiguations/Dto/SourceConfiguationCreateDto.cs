using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.SourceConfiguations.Dto
{
    public class SourceConfiguationCreateDto
    {
        public Guid BackUPTypeId { get; set; }
        public Guid? DBTypeId { get; set; }
        public string DatabaseName { get; set; }
        public string DbUsername { get; set; }
        public string DbPassword { get; set; }
        public string Port { get; set; }
        public string SshUserName { get; set; }
        public string SshPassword { get; set; }
        public string ServerIP { get; set; }
        public string DBInitialCatalog { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string PrivateKeyPath { get; set; }
        public string BackUpInitiatedPath { get; set; }
        public string Sourcepath { get; set; }
        public string OS { get; set; }
        public Guid? BackUpStorageConfiguationId { get; set; }
        public string BackupName { get; set; }
    }
}
