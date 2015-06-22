using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SoftFluent.Windows
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty WhenFalseProperty = DependencyProperty.Register("WhenFalse", typeof(Visibility), typeof(BooleanToVisibilityConverter), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty WhenTrueProperty = DependencyProperty.Register("WhenTrue", typeof(Visibility), typeof(BooleanToVisibilityConverter), new PropertyMetadata(Visibility.Visible));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return ((bool)value) ? WhenTrue : WhenFalse;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public Visibility WhenFalse
        {
            get
            {
                return (Visibility)GetValue(WhenFalseProperty);
            }
            set
            {
                SetValue(WhenFalseProperty, value);
            }
        }

        public Visibility WhenTrue
        {
            get
            {
                return (Visibility)GetValue(WhenTrueProperty);
            }
            set
            {
                SetValue(WhenTrueProperty, value);
            }
        }
    }
}