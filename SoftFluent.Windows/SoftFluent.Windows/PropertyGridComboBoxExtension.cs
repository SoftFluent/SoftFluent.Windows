using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridComboBoxExtension : MarkupExtension
    {
        private readonly Binding _binding;
        public const string DefaultZeroName = "None";

        public PropertyGridComboBoxExtension(Binding binding)
        {
            _binding = binding;
        }

        protected virtual PropertyGridItem CreateItem()
        {
            return new PropertyGridItem();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            _binding.Converter = new Converter(this);
            return _binding.ProvideValue(serviceProvider);
        }

        public virtual IEnumerable BuildItems(PropertyGridProperty property, Type targetType, object parameter, CultureInfo culture)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            Type enumType;
            bool nullable;
            bool isEnum = PropertyGridProperty.IsExtendedEnum(property.PropertyType, out enumType, out nullable);

            ObservableCollection<PropertyGridItem> items = new ObservableCollection<PropertyGridItem>();
            if (isEnum)
            {
                if (nullable)
                {
                    PropertyGridItem item = CreateItem();
                    item.Property = property;
                    item.Name = null;// "<unset>";
                    item.Value = null;
                    items.Add(item);
                }

                string[] names = Enum.GetNames(enumType);
                Array values = Enum.GetValues(enumType);
                if (Extensions.IsFlagsEnum(enumType))
                {
                    ulong uvalue = Extensions.EnumToUInt64(property.Value);
                    PropertyGridItem zero = null;

                    for (int i = 0; i < names.Length; i++)
                    {
                        string name = names[i];
                        ulong nameValue = Extensions.EnumToUInt64(values.GetValue(i));
                        string displayName;
                        if (!ShowEnumField(property, enumType, names[i], out displayName))
                            continue;

                        PropertyGridItem item = CreateItem();
                        item.Property = property;
                        item.Name = displayName;
                        item.Value = nameValue;
                        bool isChecked = true;

                        if (nameValue == 0)
                        {
                            zero = item;
                        }

                        // determine if this name is in fact a combination of other names
                        ulong bitsCount = (ulong)Extensions.GetEnumMaxPower(enumType) - 1; // skip first
                        ulong b = 1;
                        for (ulong bit = 1; bit < bitsCount; bit++) // signed, skip highest bit
                        {
                            string bitName = Enum.GetName(enumType, b);
                            if (bitName != null && name != bitName && (nameValue & b) != 0)
                            {
                                if ((uvalue & b) == 0)
                                {
                                    isChecked = false;
                                }
                            }
                            b *= 2;
                        }

                        isChecked = (uvalue & nameValue) != 0;
                        item.IsChecked = isChecked;
                        items.Add(item);
                    }

                    // determine if the lisbox is empty, which we don't want anyway
                    if (items.Count == 0)
                    {
                        PropertyGridItem item = CreateItem();
                        item.Property = property;
                        item.Name = DefaultZeroName;
                        item.Value = 0;
                        items.Add(item);
                    }

                    if (uvalue == 0 && zero != null)
                    {
                        zero.IsChecked = true;
                    }
                }
                else
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        string displayName;
                        if (!ShowEnumField(property, enumType, names[i], out displayName))
                            continue;

                        PropertyGridItem item = CreateItem();
                        item.Property = property;
                        item.Name = displayName;
                        item.Value = values.GetValue(i);
                        items.Add(item);
                    }
                }
            }
            else
            {
                PropertyGridOptionsAttribute att = PropertyGridOptionsAttribute.FromProperty(property);
                if (att != null && att.IsEnum)
                {
                    // either EnumList or EnumValues can be null but not both
                    // if not null, length must be the same
                    if (att.EnumNames == null || att.EnumNames.Length == 0)
                    {
                        if (att.EnumValues == null || att.EnumValues.Length == 0)
                            return items;

                        att.EnumNames = new string[att.EnumValues.Length];
                        for (int i = 0; i < att.EnumValues.Length; i++)
                        {
                            att.EnumNames[i] = string.Format("{0}", att.EnumValues[i]);
                        }
                    }
                    else
                    {
                        if (att.EnumValues == null || att.EnumValues.Length != att.EnumNames.Length)
                        {
                            att.EnumValues = new object[att.EnumNames.Length];
                            for (int i = 0; i < att.EnumNames.Length; i++)
                            {
                                att.EnumValues[i] = string.Format("{0}", att.EnumNames[i]);
                            }
                        }
                    }

                    if (att.IsFlagsEnum)
                    {
                        ulong uvalue = Extensions.EnumToUInt64(property.Value);

                        for (int i = 0; i < att.EnumNames.Length; i++)
                        {
                            ulong nameValue = Extensions.EnumToUInt64(att.EnumValues[i]);

                            PropertyGridItem item = CreateItem();
                            item.Property = property;
                            item.Name = att.EnumNames[i];
                            item.Value = att.EnumValues[i];
                            bool isChecked = true;

                            // determine if this name is in fact a combination of other names
                            ulong bitsCount = (ulong)Extensions.GetEnumMaxPower(property.PropertyType) - 1; // skip first
                            ulong b = 1;
                            for (ulong bit = 1; bit < bitsCount; bit++) // signed, skip highest bit
                            {
                                string bitName = Enum.GetName(property.PropertyType, b);
                                if (bitName != null && att.EnumNames[i] != bitName && (nameValue & b) != 0)
                                {
                                    if ((uvalue & b) == 0)
                                    {
                                        isChecked = false;
                                    }
                                }
                                b *= 2;
                            }

                            isChecked = (uvalue & nameValue) != 0;
                            item.IsChecked = isChecked;
                            items.Add(item);
                        }

                        // determine if the lisbox is empty, which we don't want anyway
                        if (items.Count == 0)
                        {
                            PropertyGridItem item = CreateItem();
                            item.Property = property;
                            item.Name = DefaultZeroName;
                            item.Value = 0;
                            items.Add(item);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < att.EnumNames.Length; i++)
                        {
                            PropertyGridItem item = CreateItem();
                            item.Property = property;
                            item.Name = att.EnumNames[i];
                            item.Value = att.EnumValues[i];
                            items.Add(item);
                        }
                    }
                }
            }

            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["items"] = items;
            property.OnEvent(this, new PropertyGridEventArgs(property, ctx));
            return items;
        }

        protected virtual bool ShowEnumField(PropertyGridProperty property, Type type, string name, out string displayName)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (type == null)
                throw new ArgumentNullException("type");

            if (name == null)
                throw new ArgumentNullException("name");

            FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public);
            displayName = fi.Name;
            BrowsableAttribute ba = fi.GetAttribute<BrowsableAttribute>();
            if (ba != null && !ba.Browsable)
                return false;

            DescriptionAttribute da = fi.GetAttribute<DescriptionAttribute>();
            if (da != null && !string.IsNullOrWhiteSpace(da.Description))
            {
                displayName = da.Description;
            }
            return true;
        }

        protected class Converter : IValueConverter
        {
            public Converter(PropertyGridComboBoxExtension extension)
            {
                Extension = extension;
            }

            public PropertyGridComboBoxExtension Extension { get; private set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                PropertyGridProperty property = value as PropertyGridProperty;
                if (property != null)
                    return Extension.BuildItems(property, targetType, parameter, culture);

                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }
        }
    }
}