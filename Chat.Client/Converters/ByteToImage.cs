using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Chat.Client.Converters
{
    public class ByteToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || ((byte[])value).Length == 0)
            {
                return Binding.DoNothing;
            }

            ImageSourceConverter converter = new();

            return converter.ConvertFrom(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
