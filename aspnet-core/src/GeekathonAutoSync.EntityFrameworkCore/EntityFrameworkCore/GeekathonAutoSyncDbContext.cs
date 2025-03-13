using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using GeekathonAutoSync.Authorization.Roles;
using GeekathonAutoSync.Authorization.Users;
using GeekathonAutoSync.MultiTenancy;
using GeekathonAutoSync.StorageMasterTypes;
using GeekathonAutoSync.BackUpFrequencys;
using GeekathonAutoSync.CloudStorages;
using GeekathonAutoSync.BackUpLogs;
using GeekathonAutoSync.BackUpSchedules;
using GeekathonAutoSync.BackUpStorageConfiguations;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.DBTypes;
using GeekathonAutoSync.SourceConfiguations;

namespace GeekathonAutoSync.EntityFrameworkCore
{
    public class GeekathonAutoSyncDbContext : AbpZeroDbContext<Tenant, Role, User, GeekathonAutoSyncDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public virtual DbSet<BackUpFrequency> BackupFrequencies { get; set; }
        public virtual DbSet<BackUpLog> BackUpLogs { get; set; }
        public virtual DbSet<BackUpSchedule> BackUpSchedules { get; set; }
        public virtual DbSet<BackUpStorageConfiguation> BackUpStorageConfiguations { get; set; }
        public virtual DbSet<BackUPType> BackUPTypes { get; set; }
        public virtual DbSet<CloudStorage> CloudStorages { get; set; }
        public virtual DbSet<DBType> DBTypes { get; set; }
        public virtual DbSet<SourceConfiguation> SourceConfiguations { get; set; }
        public virtual DbSet<StorageMasterType> StorageMasterTypes { get; set; }

        public GeekathonAutoSyncDbContext(DbContextOptions<GeekathonAutoSyncDbContext> options)
            : base(options)
        {
        }
    }
}
