using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SoftFluent.Windows
{
    public class EnumerableConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty MaxItemsProperty =
            DependencyProperty.Register("MaxItems", typeof(int), typeof(EnumerableConverter), new PropertyMetadata(10));
        
        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register("Separator", typeof(string), typeof(EnumerableConverter), new PropertyMetadata(", "));
        
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(EnumerableConverter), new PropertyMetadata("{0}"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string) && !(value is string) && value is IEnumerable)
            {
                StringBuilder sb = new StringBuilder();
                IEnumerable enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    foreach (object obj in enumerable)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(Separator);
                        }
                        sb.AppendFormat(Format, obj);
                    }
                }
                return sb.ToString();
            }
            return ConversionService.ChangeType(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public int MaxItems
        {
            get
            {
                return (int)GetValue(MaxItemsProperty);
            }
            set
            {
                SetValue(MaxItemsProperty, value);
            }
        }

        public string Format
        {
            get
            {
                return (string)GetValue(FormatProperty);
            }
            set
            {
                SetValue(FormatProperty, value);
            }
        }

        public string Separator
        {
            get
            {
                return (string)GetValue(SeparatorProperty);
            }
            set
            {
                SetValue(SeparatorProperty, value);
            }
        }
    }
}
