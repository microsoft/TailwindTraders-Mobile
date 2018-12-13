using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public interface IPlatformService
    {
        void KeyboardClick();

        Task<bool> ResizeImageAsync(string filePath, PhotoSize photoSize, int quality);
    }
}
