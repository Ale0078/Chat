using System;
using System.Globalization;
using System.Windows.Data;

namespace Chat.Client.Converters
{
    public class ArrayNullOrEmtyToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is null || ((Array)value).Length == 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
