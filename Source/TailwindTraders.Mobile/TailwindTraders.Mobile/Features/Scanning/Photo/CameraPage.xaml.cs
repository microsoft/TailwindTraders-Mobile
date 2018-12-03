using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public partial class CameraPage
    {
        public CameraPage()
        {
            InitializeComponent();

            BindingContext = new CameraViewModel();
        }
    }
}
