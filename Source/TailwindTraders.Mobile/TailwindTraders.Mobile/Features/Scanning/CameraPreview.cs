using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning
{
    public class CameraPreview : View
    {
        public static readonly BindableProperty EnableTensorflowAnalysisProperty = BindableProperty.Create(
            propertyName: nameof(EnableTensorflowAnalysis),
            returnType: typeof(bool),
            declaringType: typeof(CameraPreview),
            defaultValue: false);

        public bool EnableTensorflowAnalysis
        {
            get => (bool)GetValue(EnableTensorflowAnalysisProperty);
            set => SetValue(EnableTensorflowAnalysisProperty, value);
        }

        public Func<Task<string>> TakePicture;
    }
}
