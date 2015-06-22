using System;
using System.Globalization;
using System.Windows.Data;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridConverter : IValueConverter
    {
        private static Type GetParameterAsType(object parameter)
        {
            if (parameter == null)
                return null;

            string typeName = string.Format("{0}", parameter);
            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            return AssemblyUtilities.GetType(typeName, false);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type parameterType = GetParameterAsType(parameter);
            if (parameterType != null)
            {
                value = ConvertUtilities.ChangeType(value, parameterType, culture);
            }

            object convertedValue;
            if (targetType == null)
            {
                convertedValue = value;
            }
            else
            {
                convertedValue = ConvertUtilities.ChangeType(value, targetType, culture);
            }

            return convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object convertedValue;
            if (targetType == null)
            {
                convertedValue = value;
            }
            else
            {
                convertedValue = ConvertUtilities.ChangeType(value, targetType, culture);
            }

            return convertedValue;
        }
    }
}