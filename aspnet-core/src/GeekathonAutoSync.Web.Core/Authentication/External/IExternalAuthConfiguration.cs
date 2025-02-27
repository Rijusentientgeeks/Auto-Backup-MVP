using System.Collections.Generic;

namespace GeekathonAutoSync.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
