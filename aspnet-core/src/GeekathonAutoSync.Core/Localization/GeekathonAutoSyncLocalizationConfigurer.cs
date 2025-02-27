using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace GeekathonAutoSync.Localization
{
    public static class GeekathonAutoSyncLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(GeekathonAutoSyncConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(GeekathonAutoSyncLocalizationConfigurer).GetAssembly(),
                        "GeekathonAutoSync.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
