using TailwindTraders.Mobile.Framework;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConnectivityService))]

namespace TailwindTraders.Mobile.Framework
{
    internal class ConnectivityService : IConnectivityService
    {
        public bool IsThereInternet => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
