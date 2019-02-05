namespace TailwindTraders.Mobile.Features.Settings
{
    public static class DefaultSettings
    {
        public const string ApiAuthorizationHeader = "Authorization";

        public const string AnonymousToken = "Email anonymous@anonymous.anonymous";

        public const string AppCenterAndroidSecret = "ENTER_YOUR_GUID_HERE";

        public const string AppCenteriOSSecret = "ENTER_YOUR_GUID_HERE";

        public const bool UseFakeAPIs = DebugMode;

        public const bool UseFakeAuthentication = DebugMode;

        public const bool ForceAutomaticLogin =
#if IS_UI_TEST
            false;
#else
        DebugMode;
#endif

        public const bool FailRandomly =
#if IS_UI_TEST
            false;
#else
        DebugMode;
#endif

        public const bool AndroidDebuggable = DebugMode;

        public const bool UseDebugLogging = DebugMode;

        public const bool EnableARDiagnostics = DebugMode;

        public const bool DebugMode =
#if DEBUG || IS_UI_TEST
            true;
#else
            false;
#endif

        public static string RootApiUrl { get; set; } = "http://ENTER/YOUR/URL/HERE";

        public static string ProductApiUrl { get; } = $"{RootApiUrl}/products";

        public static string ProfilesApiUrl { get; } = $"{RootApiUrl}/profiles";
    }
}
