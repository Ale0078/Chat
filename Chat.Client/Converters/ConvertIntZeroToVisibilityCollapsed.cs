using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Chat.Client.Converters
{
    public class ConvertIntZeroToVisibilityCollapsed : IValueConverter
    {
        private const int ZERO = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value.Equals(ZERO)
            ? Visibility.Collapsed
            : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
