namespace TailwindTraders.Mobile.Framework
{
    public interface IConnectivityService
    {
        bool IsThereInternet { get; }
    }
}