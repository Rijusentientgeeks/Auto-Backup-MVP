using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace GeekathonAutoSync.EntityFrameworkCore
{
    public static class GeekathonAutoSyncDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<GeekathonAutoSyncDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<GeekathonAutoSyncDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
