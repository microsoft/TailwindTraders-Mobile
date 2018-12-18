using System;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public interface IPlatformService
    {
        void KeyboardClick();

        bool ResizeImage(string filePath, PhotoSize photoSize, int quality);

        string CopyToFilesAndGetPath(string path);

        void ReadImageFileToTensor(byte[] imageData, bool quantized, IntPtr dest, int inputHeight, int inputWidth);
    }
}
