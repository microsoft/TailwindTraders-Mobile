using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Scanning.Photo
{
    public class ExtendedLabel : Label
    {
        public static readonly BindableProperty ExtendedLineHeightProperty = BindableProperty.Create(
            nameof(ExtendedLineHeight), 
            typeof(double), 
            typeof(ExtendedLabel),
            -1.0d);

        public double ExtendedLineHeight
        {
            get { return (double)GetValue(ExtendedLineHeightProperty); }
            set { SetValue(ExtendedLineHeightProperty, value); }
        }

        public static readonly BindableProperty ExtendedMaxLinesProperty = BindableProperty.Create(
            nameof(ExtendedMaxLines),
            typeof(int), 
            typeof(ExtendedLabel),
            -1);

        public int ExtendedMaxLines
        {
            get { return (int)GetValue(ExtendedMaxLinesProperty); }
            set { SetValue(ExtendedMaxLinesProperty, value); }
        }
    }
}
