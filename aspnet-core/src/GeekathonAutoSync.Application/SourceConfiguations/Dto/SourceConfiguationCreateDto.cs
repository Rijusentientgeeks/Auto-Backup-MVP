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
        public string ServerIP { get; set; }
        public string DBInitialCatalog { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string PrivateKeyPath { get; set; }
        public string BackUpInitiatedPath { get; set; }
        public string Sourcepath { get; set; }
        public string OS { get; set; }
        public Guid? BackUpStorageConfiguationId { get; set; }
    }
}
