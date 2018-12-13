using System;
using Xamarin.UITest;

namespace TailwindTraders.UITests
{
    static class AppManager
    {
        const string ApkPath = "../../../Binaries/com.microsoft.tailwindtraders-Signed.apk";
        const string AppPath = "../../../Binaries/TailwindTraders.Mobile.iOS.app";
        const string IpaBundleId = "com.microsoft.TailwindTraders-df";

        static IApp app;
        public static IApp App
        {
            get
            {
                if (app == null)
                    throw new NullReferenceException("'AppManager.App' not set. Call 'AppManager.StartApp()' before trying to access it.");
                return app;
            }
        }

        static Platform? platform;
        public static Platform Platform
        {
            get
            {
                if (platform == null)
                    throw new NullReferenceException("'AppManager.Platform' not set.");
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
                    // Used to run a .apk file:
                    .ApkFile(ApkPath)
                    .StartApp();
            }

            if (Platform == Platform.iOS)
            {
                app = ConfigureApp
                    .iOS
                    // Used to run a .app file on an ios simulator:
                    .AppBundle(AppPath)
                    // Used to run a .ipa file on a physical ios device:
                    //.InstalledApp(IpaBundleId)
                    .StartApp();
            }
        }
    }
}
