namespace TailwindTraders.Mobile.Features.Settings
{
    public static class DefaultSettings
    {
        public const string ApiAuthorizationHeader = "Authorization";

        public const string AnonymousToken = "Email anonymous@anonymous.anonymous";

        public static string RootApiUrl { get; set; } = "http://ENTER/YOUR/URL/HERE";

        public static string ProductApiUrl { get; } = $"{RootApiUrl}/products";

        public static string ProfilesApiUrl { get; } = $"{RootApiUrl}/profiles";

        public const string AppCenterAndroidSecret = "ENTER_YOUR_GUID_HERE";

        public const string AppCenteriOSSecret = "ENTER_YOUR_GUID_HERE";

        public static bool UseFakeAPIs = DebugMode;

        public static bool UseFakeAuthentication = DebugMode;

        public static bool UseDebugLogging = DebugMode;

        public static bool ForceAutomaticLogin = DebugMode;

        public static bool AndroidDebuggable = DebugMode;

        public static bool DebugMode =
#if DEBUG
            true;
#else
            false;
#endif
    }
}
