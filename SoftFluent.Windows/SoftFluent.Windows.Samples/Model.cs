using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Addresses = new List<Address>();
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

        public List<Address> Addresses
        {
            get { return GetProperty<List<Address>>(); }
            set { SetProperty<List<Address>>(value); }
        }
    }

    public class Address : AutoObject
    {
        public string Line1
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
        public string Line2
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
        public string ZipCode
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
        public string City
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
        public string Country
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
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
}
