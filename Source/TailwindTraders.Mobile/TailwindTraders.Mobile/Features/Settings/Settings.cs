namespace TailwindTraders.Mobile.Features.Settings
{
    public static class Settings
    {
        public const string ApiAuthorizationHeader = "Authorization";

        public static string AnonymousToken { get; } = "Email anonymous@anonymous.anonymous";

        public static string RootApiUrl { get; } = "ENTER_YOUR_URL_HERE";

        public static string ProductApiUrl { get; } = $"{RootApiUrl}/products";

        public static string ProfilesApiUrl { get; } = $"{RootApiUrl}/profiles";

        public static string AppCenterAndroidSecret { get; } = "ENTER_YOUR_GUID_HERE";

        public static string AppCenteriOSSecret { get; } = "ENTER_YOUR_GUID_HERE";

        public static bool UseFakeAPIs { get; } =
#if DEBUG
            true;
#else
            false;
#endif

        public static bool UseFakeAuthentication { get; } =
#if DEBUG
            true;
#else
            false;
#endif
    }
}
