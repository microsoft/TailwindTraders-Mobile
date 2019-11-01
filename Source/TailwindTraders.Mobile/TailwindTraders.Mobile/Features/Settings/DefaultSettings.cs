namespace TailwindTraders.Mobile.Features.Settings
{
    public static class DefaultSettings
    {
        public const string ApiAuthorizationHeader = "Authorization";

        public static string AccessToken = string.Empty;

        public const string AppCenterAndroidSecret = "__ENTER_YOUR_ANDROID_APPCENTER_SECRET_HERE__";

        public const string AppCenteriOSSecret = "__ENTER_YOUR_IOS_APPCENTER_SECRET_HERE__";

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

        public static string RootApiUrl { get; set; } = "__ENTER_YOUR_HTTPS_ROOT_API_URL_HERE__";

        public static string RootWebApiUrl
        { get; set; } = "__ENTER_YOUR_HTTPS_WEBBFF_ROOT_API_URL_HERE__";
    }
}
