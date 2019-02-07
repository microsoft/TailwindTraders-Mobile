namespace TailwindTraders.Mobile.Features.Settings
{
    public static class DefaultSettings
    {
        public const string ApiAuthorizationHeader = "Authorization";

        public const string AnonymousToken = "Email anonymous@anonymous.anonymous";

        public const string AppCenterAndroidSecret = "ENTER_YOUR_GUID_HERE";

        public const string AppCenteriOSSecret = "ENTER_YOUR_GUID_HERE";

        public const bool UseFakeAPIs = UITestMode || DebugMode;

        public const bool UseFakeAuthentication = UITestMode || DebugMode;

        public const bool ForceAutomaticLogin = !UITestMode && DebugMode;

        public const bool BreakNetworkRandomly = !UITestMode && DebugMode;

        public const bool AndroidDebuggable = DebugMode;

        public const bool UseDebugLogging = UITestMode || DebugMode;

        public const bool EnableARDiagnostics = DebugMode;

        public const bool DebugMode =
#if DEBUG 
            true;
#else
            false;
#endif

        public const bool UITestMode =
#if IS_UI_TEST
            true;
#else
            false;
#endif

        public static string RootApiUrl { get; set; } = "http://ENTER/YOUR/URL/HERE";

        public static string ProductApiUrl { get; } = $"{RootApiUrl}/products";

        public static string ProfilesApiUrl { get; } = $"{RootApiUrl}/profiles";
    }
}
