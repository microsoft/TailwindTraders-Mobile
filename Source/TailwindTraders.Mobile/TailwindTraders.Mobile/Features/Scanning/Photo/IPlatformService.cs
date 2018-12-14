using System;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public interface IPlatformService
    {
        void KeyboardClick();

        Task<bool> ResizeImageAsync(string filePath, PhotoSize photoSize, int quality);

        string GetContent(string path);

        string CopyToFilesAndGetPath(string path);

        void ReadImageFileToTensor(string fileName, bool quantized, IntPtr dest, int inputHeight, int inputWidth);
    }
}
