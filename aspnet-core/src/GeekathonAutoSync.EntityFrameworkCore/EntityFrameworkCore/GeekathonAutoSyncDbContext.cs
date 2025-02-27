using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using GeekathonAutoSync.Authorization.Roles;
using GeekathonAutoSync.Authorization.Users;
using GeekathonAutoSync.MultiTenancy;

namespace GeekathonAutoSync.EntityFrameworkCore
{
    public class GeekathonAutoSyncDbContext : AbpZeroDbContext<Tenant, Role, User, GeekathonAutoSyncDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public GeekathonAutoSyncDbContext(DbContextOptions<GeekathonAutoSyncDbContext> options)
            : base(options)
        {
        }
    }
}
