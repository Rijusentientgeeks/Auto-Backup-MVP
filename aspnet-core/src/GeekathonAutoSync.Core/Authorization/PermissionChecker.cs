using Abp.Authorization;
using GeekathonAutoSync.Authorization.Roles;
using GeekathonAutoSync.Authorization.Users;

namespace GeekathonAutoSync.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
