using System.Windows.Input;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public class GalleryImageViewModel
    {
        public GalleryImageViewModel(string imageUrl, Command<string> tappedCommand)
        {
            ImageUrl = imageUrl;
            TappedCommand = tappedCommand;
        }

        public string ImageUrl { get; }

        public ICommand TappedCommand { get; }
    }
}
