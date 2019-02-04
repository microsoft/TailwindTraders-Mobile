namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public partial class CameraPage
    {
        public CameraPage(string mediaPath)
        {
            InitializeComponent();

            BindingContext = new CameraViewModel(mediaPath);
        }
    }
}
