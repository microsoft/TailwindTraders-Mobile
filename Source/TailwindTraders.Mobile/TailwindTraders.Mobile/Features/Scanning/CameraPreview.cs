using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning
{
    public class CameraPreview : View
    {
        public static readonly BindableProperty CameraProperty = BindableProperty.Create(
            propertyName: nameof(Camera),
            returnType: typeof(CameraOptions),
            declaringType: typeof(CameraPreview),
            defaultValue: CameraOptions.Rear);

        public CameraOptions Camera
        {
            get => (CameraOptions)GetValue(CameraProperty);
            set => SetValue(CameraProperty, value);
        }

        public Func<Task<string>> TakePicture;
    }
}
