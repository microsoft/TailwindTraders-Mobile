using System;
using Xamarin.UITest;

namespace TailwindTraders.UITests
{
    internal static class AppManager
    {
        private const string ApkPath = "../../../Binaries/com.microsoft.tailwindtraders-Signed.apk";
        private const string AppPath = "../../../Binaries/TailwindTraders.Mobile.iOS.app";
        private const string IpaBundleId = "com.microsoft.TailwindTraders-df";

        private static IApp app;

        public static IApp App
        {
            get
            {
                if (app == null)
                {
                    throw new NullReferenceException(
                        "'AppManager.App' not set. Call 'AppManager.StartApp()' before trying to access it.");
                }

                return app;
            }
        }

        private static Platform? platform;

        public static Platform Platform
        {
            get
            {
                if (platform == null)
                {
                    throw new NullReferenceException("'AppManager.Platform' not set.");
                }

                return platform.Value;
            }

            set
            {
                platform = value;
            }
        }

        public static void StartApp()
        {
            if (Platform == Platform.Android)
            {
                app = ConfigureApp
                    .Android
                    .ApkFile(ApkPath) // Used to run a .apk file
                    .StartApp();
            }

            if (Platform == Platform.iOS)
            {
                app = ConfigureApp
                    .iOS
                    .AppBundle(AppPath) // Used to run a .app file on an ios simulator

                    // .InstalledApp(IpaBundleId) // Used to run a .ipa file on a physical ios device
                    .StartApp();
            }
        }
    }
}
