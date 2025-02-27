using GeekathonAutoSync.Debugging;

namespace GeekathonAutoSync
{
    public class GeekathonAutoSyncConsts
    {
        public const string LocalizationSourceName = "GeekathonAutoSync";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "a7d4d8b4f27f4430ab0a2fdea86372af";
    }
}
