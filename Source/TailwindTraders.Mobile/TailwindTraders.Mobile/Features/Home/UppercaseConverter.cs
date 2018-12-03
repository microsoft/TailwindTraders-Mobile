using System;
using System.Globalization;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Home
{
    internal class UppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string output;

            if (value is string input)
            {
                output = input?.ToUpperInvariant();
            }
            else
            {
                throw new ArgumentException("Value must be string.", nameof(value));
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
