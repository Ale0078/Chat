using System;
using System.Globalization;
using System.Windows.Data;

namespace Chat.Client.Converters
{
    public class IsIntEqualsToZero : IValueConverter
    {
        private const int ZERO = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value.Equals(ZERO);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
