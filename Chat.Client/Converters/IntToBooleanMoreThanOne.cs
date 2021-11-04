using System;
using System.Globalization;
using System.Windows.Data;

namespace Chat.Client.Converters
{
    public class IntToBooleanMoreThanOne : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value) > 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
