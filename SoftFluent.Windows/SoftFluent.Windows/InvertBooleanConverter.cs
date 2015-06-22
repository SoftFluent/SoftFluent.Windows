#if NETFX_CORE
using Windows.UI.Xaml.Data;
#else
#endif
using System;
using System.Globalization;
using System.Windows.Data;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{
    /// <summary>
    /// Provides a type converter to convert any object into the invert boolean value. If parameter is provided, it will test if value is diferrent from to parameter.
    /// </summary>
    public class InvertBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return parameter != null;

            if (parameter == null)
                return !ConvertUtilities.ChangeType(value, false, culture);

            object typedParameter = ConvertUtilities.ChangeType(parameter, value.GetType(), culture);
            return !value.Equals(typedParameter);
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return !ConvertUtilities.ChangeType(value, false, culture);

            return ConvertUtilities.ChangeType(parameter, targetType, culture);
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, UniversalConverter.CultureInfoFromName(language));
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertBack(value, targetType, parameter, UniversalConverter.CultureInfoFromName(language));
        }
    }
}
