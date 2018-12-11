using System;
using Android.App;
using Android.Runtime;
using Plugin.CurrentActivity;

[assembly: Xamarin.Forms.ResolutionGroupName(nameof(TailwindTraders))]

namespace TailwindTraders.Mobile.Droid
{
    [Application(Debuggable = Mobile.Features.Settings.Settings.AndroidDebuggable)]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            CrossCurrentActivity.Current.Init(this);
        }
    }
}