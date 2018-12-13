using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Common
{
    public interface IPlatformService
    {
        void KeyboardClick();

        Task<bool> ResizeImageAsync(string filePath, PhotoSize photoSize, int quality);
    }
}
