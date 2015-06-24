using System;
using System.Globalization;
using System.Windows.Data;

namespace SoftFluent.Windows
{
    public class ChangeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConversionService.ChangeType(value, targetType, null, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
