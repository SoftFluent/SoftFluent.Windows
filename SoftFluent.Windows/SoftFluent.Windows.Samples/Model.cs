using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Media;

namespace SoftFluent.Windows.Samples
{
    public class Customer : AutoObject
    {
        public Customer()
        {
            Id = Guid.NewGuid();
            ListOfStrings = new List<string>();
            ListOfStrings.Add("string 1");
            ListOfStrings.Add("string 2");

            ArrayOfStrings = ListOfStrings.ToArray();
            CreationDateAndTime = DateTime.Now;
            Description = "press button to edit...";
            ByteArray1 = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            WebSite = "http://www.softfluent.com";
            Status = Status.Valid;
            Addresses = new ObservableCollection<Address> { new Address { Line1 = "2018 156th Avenue NE", City = "Bellevue", State = "WA", ZipCode = 98007, Country = "USA" } };
            DaysOfWeek = DaysOfWeek.WeekDays;
            PercentageOfSatisfaction = 50;
            PreferredColorName = "DodgerBlue";
            PreferredFont = Fonts.SystemFontFamilies.FirstOrDefault(f => string.Equals(f.Source, "Consolas", StringComparison.OrdinalIgnoreCase));
            SampleNullableBooleanDropDownList = false;
            SampleBooleanDropDownList = true;
            MultiEnumString = "First, Second";
            SubObject = Address.Parse("1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA");
        }

        [DisplayName("Guid (see menu on right-click)")]
        public Guid Id
        {
            get { return GetProperty<Guid>(); }
            set { SetProperty(value); }
        }

        //[ReadOnly(true)]
        [Category("Dates and Times")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "DateTimePicker")]
        public DateTime CreationDateAndTime
        {
            get { return GetProperty<DateTime>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Sub Object (Address)")]
        [PropertyGridOptions(ForcePropertyChanged = true)]
        public Address SubObject
        {
            get { return GetProperty<Address>(); }
            set
            {
                // because it's a sub object we want to update the property grid
                // when inner properties change
                var so = SubObject;
                if (so != null)
                {
                    so.PropertyChanged -= OnSubObjectPropertyChanged;
                }

                if (SetProperty(value))
                {
                    so = SubObject;
                    if (so != null)
                    {
                        so.PropertyChanged += OnSubObjectPropertyChanged;
                    }

                    // these two properties are coupled
                    OnPropertyChanged(nameof(SubObjectAsObject));
                }
            }
        }

        private void OnSubObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SubObject), false, true);
            OnPropertyChanged(nameof(SubObjectAsObject), false, true);
        }

        [DisplayName("Sub Object (Address as Object)")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor", ForcePropertyChanged = true)]
        public Address SubObjectAsObject
        {
            get { return SubObject; }
            set { SubObject = value; }
        }

        [PropertyGridOptions(SortOrder = 10)]
        public string FirstName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(SortOrder = 20)]
        public string LastName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [Category("Dates and Times")]
        [PropertyGridOptions(SortOrder = 40)]
        public DateTime DateOfBirth
        {
            get { return GetProperty<DateTime>(); }
            set { SetProperty(value); }
        }

        [Category("Enums")]
        [PropertyGridOptions(SortOrder = 30)]
        public Gender Gender
        {
            get { return GetProperty<Gender>(); }
            set { SetProperty(value); }
        }

        [Category("Enums")]
        public Status Status
        {
            get { return GetProperty<Status>(); }
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(StatusColorString));
                }
            }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEnumEditor", PropertyType = typeof(PropertyGridEnumProperty))]
        [DisplayName("Status (colored enum)")]
        [ReadOnly(true)]
        [Category("Enums")]
        public Status StatusColor
        {
            get { return Status; }
            set { Status = value; }
        }

        [PropertyGridOptions(IsEnum = true, EnumNames =  new string [] {"1N\\/AL1D", "\\/AL1D", "UNKN0WN"}, EnumValues = new object[] { Status.Invalid, Status.Valid, Status.Unknown })]
        [DisplayName("Status (enum as string list)")]
        [Category("Enums")]
        public string StatusColorString
        {
            get { return Status.ToString(); }
            set { Status = (Status)Enum.Parse(typeof(Status), value); }
        }

        [PropertyGridOptions(IsEnum = true, IsFlagsEnum = true, EnumNames = new string[] { "First", "Second", "Third" })]
        [Category("Enums")]
        public string MultiEnumString
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(IsEnum = true, IsFlagsEnum = true, EnumNames = new string[] { "None", "My First", "My Second", "My Third" }, EnumValues = new object[] { 0, 1, 2, 4 })]
        [Category("Enums")]
        public string MultiEnumStringWithDisplay
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [Category("Dates and Times")]
        public TimeSpan TimeSpan
        {
            get { return GetProperty<TimeSpan>(); }
            set { SetProperty(value); }
        }

        [Category("Security")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "PasswordEditor")]
        [DisplayName("Password (SecureString)")]
        public SecureString Password
        {
            get { return GetProperty<SecureString>(); }
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(PasswordString));
                }
            }
        }

        [Category("Security")]
        [DisplayName("Password (String)")]
        public string PasswordString
        {
            get
            {
                if (Password == null)
                    return null;

                return Password.ConvertToUnsecureString();
            }
        }

        [Browsable(false)]
        public string NotBrowsable
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Description (multi-line)")]
        [PropertyGrid(Name = "Foreground", Value = "White")]
        [PropertyGrid(Name = "Background", Value = "Black")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "BigTextEditor")]
        public string Description
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "FormatTextEditor")]
        [PropertyGrid(Name = "Format", Value = "0x{0}")]
        [ReadOnly(true)]
        [DisplayName("Byte Array (hex format)")]
        public byte[] ByteArray1
        {
            get { return GetProperty<byte[]>(); }
            set { SetProperty(value); }
        }

        [ReadOnly(true)]
        [DisplayName("Byte Array (press button for hex dump)")]
        public byte[] ByteArray2
        {
            get { return ByteArray1; }
            set { ByteArray1 = value; }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "CustomEditor", SortOrder = -10)]
        [DisplayName("Web Site (custom sort order)")]
        public string WebSite
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [Category("Collections")]
        public string[] ArrayOfStrings
        {
            get { return GetProperty<string[]>(); }
            set { SetProperty(value); }
        }

        [Category("Collections")]
        public List<string> ListOfStrings
        {
            get { return GetProperty<List<string>>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "AddressListEditor", SortOrder = 10)]
        [DisplayName("Addresses (custom editor)")]
        [Category("Collections")]
        public ObservableCollection<Address> Addresses
        {
            get { return GetProperty<ObservableCollection<Address>>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Days Of Week (multi-valued enum)")]
        [Category("Enums")]
        public DaysOfWeek DaysOfWeek
        {
            get { return GetProperty<DaysOfWeek>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "PercentEditor")]
        [DisplayName("Percentage Of Satisfaction (int)")]
        public int PercentageOfSatisfactionInt
        {
            get { return GetProperty<int>(0, "PercentageOfSatisfaction"); }
            set { SetProperty("PercentageOfSatisfaction", value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "PercentEditor")]
        [DisplayName("Percentage Of Satisfaction (double)")]
        public double PercentageOfSatisfaction
        {
            get { return GetProperty<double>(); }
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(PercentageOfSatisfactionInt));
                }
            }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEditor")]
        [DisplayName("Preferred Color Name (custom editor)")]
        public string PreferredColorName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "FontEditor")]
        [DisplayName("Preferred Font (custom editor)")]
        public FontFamily PreferredFont
        {
            get { return GetProperty<FontFamily>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Point (auto type converter)")]
        public Point Point
        {
            get { return GetProperty<Point>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Nullable Int32 (supports empty string)")]
        public int? NullableInt32
        {
            get { return GetProperty<int?>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Boolean (Checkbox)")]
        [Category("Booleans")]
        public bool SampleBoolean
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Boolean (Checkbox three states)")]
        [Category("Booleans")]
        public bool? SampleNullableBoolean
        {
            get { return GetProperty<bool?>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Boolean (DropDownList)")]
        [Category("Booleans")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "BooleanDropDownListEditor")]
        public bool SampleBooleanDropDownList
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        [DisplayName("Boolean (DropDownList 3 states)")]
        [Category("Booleans")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "NullableBooleanDropDownListEditor")]
        public bool? SampleNullableBooleanDropDownList
        {
            get { return GetProperty<bool?>(); }
            set { SetProperty(value); }
        }
    }

    [TypeConverter(typeof(AddressConverter))]
    public class Address : AutoObject
    {
        [PropertyGridOptions(SortOrder = 10)]
        public string Line1
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(SortOrder = 20)]
        public string Line2
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(SortOrder = 30)]
        public int? ZipCode
        {
            get { return GetProperty<int?>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(SortOrder = 40)]
        public string City
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(SortOrder = 50)]
        public string State
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [PropertyGridOptions(SortOrder = 60)]
        public string Country
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        protected override bool OnPropertyChanged(string name, bool setChanged, bool forceRaise)
        {
            return base.OnPropertyChanged(name, setChanged, forceRaise);
        }

        // poor man's one line comma separated USA postal address parser
        public static Address Parse(string text)
        {
            var address = new Address();
            if (text != null)
            {
                string[] split = text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {
                    int zip = 0;
                    int index = -1;
                    string state = null;
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (TryFindStateZip(split[i], out state, out zip))
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index < 0)
                    {
                        address.DistributeOverProperties(split, 0, int.MaxValue, nameof(Line1), nameof(Line2), nameof(City), nameof(Country));
                    }
                    else
                    {
                        address.ZipCode = zip;
                        address.State = state;
                        address.DistributeOverProperties(split, 0, index, nameof(Line1), nameof(Line2), nameof(City));
                        if (string.IsNullOrWhiteSpace(address.City) && address.Line2 != null)
                        {
                            address.City = address.Line2;
                            address.Line2 = null;
                        }
                        address.DistributeOverProperties(split, index + 1, int.MaxValue, nameof(Country));
                    }
                }
            }
            return address;
        }

        private static bool TryFindStateZip(string text, out string state, out int zip)
        {
            zip = 0;
            state = null;
            string zipText = text;
            int pos = text.LastIndexOfAny(new[] { ' ' });
            if (pos >= 0)
            {
                zipText = text.Substring(pos + 1).Trim();
            }

            if (!int.TryParse(zipText, out zip) || zip <= 0)
                return false;

            state = text.Substring(0, pos).Trim();
            return true;            
        }

        private void DistributeOverProperties(string[] split, int offset, int max, params string[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if ((offset + i) >= split.Length || (offset + i) >= max)
                    return;

                string s = split[offset + i].Trim();
                if (s.Length == 0)
                    continue;

                SetProperty(properties[i], (object)s);
            }
        }

        public override string ToString()
        {
            const string sep = ", ";
            var sb = new StringBuilder();
            AppendJoin(sb, Line1, string.Empty);
            AppendJoin(sb, Line2, sep);
            AppendJoin(sb, City, sep);
            AppendJoin(sb, State, sep);
            if (ZipCode.HasValue)
            {
                AppendJoin(sb, ZipCode.Value.ToString(), " ");
            }
            AppendJoin(sb, Country, sep);
            return sb.ToString();
        }

        private static void AppendJoin(StringBuilder sb, string value, string sep)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            string s = sb.ToString();
            if (!s.EndsWith(" ") && !s.EndsWith(",") && !s.EndsWith(Environment.NewLine))
            {
                sb.Append(sep);
            }
            sb.Append(value);
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
        [PropertyGrid(Name = "Foreground", Value = "Black")]
        [PropertyGrid(Name = "Background", Value = "Orange")]
        Unknown,
        [PropertyGrid(Name = "Foreground", Value = "White")]
        [PropertyGrid(Name = "Background", Value = "Red")]
        Invalid,
        [PropertyGrid(Name = "Foreground", Value = "White")]
        [PropertyGrid(Name = "Background", Value = "Green")]
        Valid
    }

    public class AddressConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
                return Address.Parse(s);

            return base.ConvertFrom(context, culture, value);
        }
    }

    public class PointConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
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
                return ((Point)value).X + ";" + ((Point)value).Y;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(PointConverter))]
    public struct Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }
    }
}
