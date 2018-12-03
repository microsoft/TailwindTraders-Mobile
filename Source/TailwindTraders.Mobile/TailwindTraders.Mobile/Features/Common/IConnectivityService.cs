namespace TailwindTraders.Mobile.Features.Common
{
    public interface IConnectivityService
    {
        bool IsThereInternet { get; }
    }
}