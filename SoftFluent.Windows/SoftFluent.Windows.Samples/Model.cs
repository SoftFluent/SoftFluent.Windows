using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows.Samples
{
    public class Customer : AutoObject
    {
        public Customer()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            WebSite = "http://www.softfluent.com";
            Status = Status.Validated;
            Addresses = new ObservableCollection<Address> { new Address { Line1 = "2018 156th Avenue NE", City = "Bellevue, WA", ZipCode = "98007", Country = "USA" } };
            ContactDays = DaysOfWeek.WeekDays;
            PercentageOfSatisfaction = 50;
            PreferedColorName = "DodgerBlue";
            PreferedFont = Fonts.SystemFontFamilies.FirstOrDefault(f => string.Equals(f.Source, "Consolas", StringComparison.OrdinalIgnoreCase));
        }

        public Guid Id
        {
            get { return GetProperty<Guid>(); }
            set { SetProperty<Guid>(value); }
        }

        [ReadOnly(true)]
        public DateTime CreationDate
        {
            get { return GetProperty<DateTime>(); }
            set { SetProperty<DateTime>(value); }
        }

        [Category("Identity")]
        [PropertyGridOptions(SortOrder = 10)]
        public string FirstName
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [Category("Identity")]
        [PropertyGridOptions(SortOrder = 20)]
        public string LastName
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [Category("Identity")]
        [PropertyGridOptions(SortOrder = 40)]
        public DateTime DateOfBirth
        {
            get { return GetProperty<DateTime>(); }
            set { SetProperty<DateTime>(value); }
        }

        [Category("Identity")]
        [PropertyGridOptions(SortOrder = 30)]
        public Gender Gender
        {
            get { return GetProperty<Gender>(); }
            set { SetProperty<Gender>(value); }
        }

        public Status Status
        {
            get { return GetProperty<Status>(); }
            set { SetProperty<Status>(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEnumEditor", PropertyType = typeof(PropertyGridEnumProperty))]
        [PropertyGrid(Name = "Foreground", Value = "Black")]
        [PropertyGrid(Name = "Background", Value = "White")]
        public Status StatusColor
        {
            get { return GetProperty<Status>(); }
            set { SetProperty<Status>(value); }
        }

        [Category("Security")]
        public string Password
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [Category("Security")]
        [Browsable(false)]
        public string NotBrowsable
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }


        [DisplayName("Description")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "BigTextEditor")]
        public string Description
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEnumEditor")]
        [PropertyGrid(Name = "Foreground", Value = "Blue")]
        [PropertyGrid(Name = "Background", Value = "Red")]
        public Color Color
        {
            get { return GetProperty<Color>(); }
            set { SetProperty<Color>(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ByteArrayHexaEditor")]
        [ReadOnly(true)]
        public byte[] RowVersion
        {
            get { return GetProperty<byte[]>(); }
            set { SetProperty<byte[]>(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "CustomEditor", SortOrder = -10)]
        public string WebSite
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "AddressListEditor", SortOrder = 10)]
        public ObservableCollection<Address> Addresses
        {
            get { return GetProperty<ObservableCollection<Address>>(); }
            set { SetProperty<ObservableCollection<Address>>(value); }
        }

        public DaysOfWeek ContactDays
        {
            get { return GetProperty<DaysOfWeek>(); }
            set { SetProperty<DaysOfWeek>(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "PercentEditor")]
        public double PercentageOfSatisfaction
        {
            get { return GetProperty<double>(); }
            set
            {
                if (SetProperty<double>(value))
                {
                    OnPropertyChanged("PercentageOfSatisfactionText");
                }
            }
        }

        //public string PercentageOfSatisfactionText
        //{
        //    get { return PercentageOfSatisfaction.ToString(); }
        //}

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEditor")]
        public string PreferedColorName
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "FontEditor")]
        public FontFamily PreferedFont
        {
            get { return GetProperty<FontFamily>(); }
            set { SetProperty<FontFamily>(value); }
        }


        //[PropertyGridOptions(EditorDataTemplateResourceKey = "FontEditor")]
        public Point Point
        {
            get { return GetProperty<Point>(); }
            set { SetProperty<Point>(value); }
        }
    }

    public class Address : AutoObject
    {
        [PropertyGridOptions(SortOrder = 10)]
        public string Line1
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [PropertyGridOptions(SortOrder = 20)]
        public string Line2
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [PropertyGridOptions(SortOrder = 30)]
        public string ZipCode
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [PropertyGridOptions(SortOrder = 40)]
        public string City
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [PropertyGridOptions(SortOrder = 50)]
        public string Country
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Line1);
            if (!string.IsNullOrEmpty(Line2))
            {
                sb.AppendLine(Line2);
            }

            sb.Append(City);
            sb.Append(" ");
            sb.AppendLine(ZipCode);
            sb.Append(Country);
            return sb.ToString();
        }
    }

    [Flags]
    public enum DaysOfWeek
    {
        NoDay = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
        WeekDays = Monday | Tuesday | Wednesday | Thursday | Friday
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Status
    {
        Unknown,
        [PropertyGrid(Name = "Foreground", Value = "Black")]
        [PropertyGrid(Name = "Background", Value = "Orange")]
        Registered,
        [PropertyGrid(Name = "Foreground", Value = "Black")]
        [PropertyGrid(Name = "Background", Value = "Green")]
        Validated
    }

    public class PointConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                string[] v = s.Split(new[] { ';' });
                return new Point(int.Parse(v[0]), int.Parse(v[1]));
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((Point)value).X + ";" + ((Point)value).Y;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(PointConverter))]
    public struct Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
    }
}
