﻿using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace GeekathonAutoSync.Authorization
{
    public class GeekathonAutoSyncAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission(PermissionNames.Pages_BackupStorageConfiguration, L("BackupStorageConfiguration"));
            context.CreatePermission(PermissionNames.Pages_SourceConfiguation, L("SourceConfiguation"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, GeekathonAutoSyncConsts.LocalizationSourceName);
        }
    }
}
