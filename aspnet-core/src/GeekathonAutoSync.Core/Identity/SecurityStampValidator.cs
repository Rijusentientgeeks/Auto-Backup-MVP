using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using GeekathonAutoSync.Authorization.Roles;
using GeekathonAutoSync.Authorization.Users;
using GeekathonAutoSync.MultiTenancy;
using Microsoft.Extensions.Logging;
using Abp.Domain.Uow;

namespace GeekathonAutoSync.Identity
{
    public class SecurityStampValidator : AbpSecurityStampValidator<Tenant, Role, User>
    {
        public SecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            SignInManager signInManager,
            ILoggerFactory loggerFactory,
            IUnitOfWorkManager unitOfWorkManager)
            : base(options, signInManager, loggerFactory, unitOfWorkManager)
        {
        }
    }
}
