using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridDataProvider : IListSource
    {
        public PropertyGridDataProvider(PropertyGrid grid, object data)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (data == null)
                throw new ArgumentNullException("data");

            Grid = grid;
            Data = data;
            Properties = new ObservableCollection<PropertyGridProperty>();
            ScanProperties();
        }

        public PropertyGrid Grid { get; private set; }
        public object Data { get; private set; }
        public ObservableCollection<PropertyGridProperty> Properties { get; private set; }

        public static bool HasProperties(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(type))
            {
                if (!descriptor.IsBrowsable)
                    continue;

                return true;
            }
            return false;
        }

        public virtual PropertyGridProperty AddProperty(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            PropertyGridProperty prop = Properties.FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
                return prop;

            PropertyDescriptor desc = TypeDescriptor.GetProperties(Data).OfType<PropertyDescriptor>().FirstOrDefault(p => p.Name == propertyName);
            if (desc != null)
            {
                prop = CreateProperty(desc);
                if (prop != null)
                {
                    Properties.Add(prop);
                }
            }
            return prop;
        }

        protected virtual PropertyGridProperty CreateProperty()
        {
            return new PropertyGridProperty(this);
        }

        protected virtual void Describe(PropertyGridProperty property, PropertyDescriptor descriptor)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            property.Descriptor = descriptor;
            property.Name = descriptor.Name;
            property.PropertyType = descriptor.PropertyType;

            // unset by default. conversion service does the default job
            //property.Converter = descriptor.Converter;

            property.Category = string.IsNullOrWhiteSpace(descriptor.Category) || descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category) ? Grid.DefaultCategoryName : descriptor.Category;
            property.IsReadOnly = descriptor.IsReadOnly;
            property.Description = descriptor.Description;
            property.DisplayName = descriptor.DisplayName;
            if (property.DisplayName == descriptor.Name)
            {
                property.DisplayName = DecamelizationService.Decamelize(property.DisplayName);
            }

            property.IsFlagsEnum = descriptor.PropertyType.IsEnum && Extensions.IsFlagsEnum(descriptor.PropertyType);

            DefaultValueAttribute att = descriptor.GetAttribute<DefaultValueAttribute>();
            property.HasDefaultValue = att != null;
            property.DefaultValue = att != null ? att.Value : null;

            PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();
            if (options != null)
            {
                if (options.SortOrder != 0)
                {
                    property.SortOrder = options.SortOrder;
                }
            }

            AddDynamicProperties(descriptor.Attributes.OfType<PropertyGridAttribute>(), property.Attributes);
            AddDynamicProperties(descriptor.PropertyType.GetAttributes<PropertyGridAttribute>(), property.TypeAttributes);
        }

        public static void AddDynamicProperties(IEnumerable<PropertyGridAttribute> attributes, DynamicObject dynamicObject)
        {
            if (attributes == null || dynamicObject == null)
                return;

            foreach (PropertyGridAttribute pga in attributes)
            {
                if (string.IsNullOrWhiteSpace(pga.Name))
                    continue;

                DynamicObjectProperty prop = dynamicObject.AddProperty(pga.Name, pga.Type, null);
                prop.SetValue(dynamicObject, pga.Value);
            }
        }

        public virtual PropertyGridProperty CreateProperty(PropertyDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            bool forceReadWrite = false;
            PropertyGridProperty property = null;
            PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();
            if (options != null)
            {
                forceReadWrite = options.ForceReadWrite;
                if (options.PropertyType != null)
                {
                    property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, this);
                }
            }

            if (property == null)
            {
                options = descriptor.PropertyType.GetAttribute<PropertyGridOptionsAttribute>();
                if (options != null)
                {
                    if (!forceReadWrite)
                    {
                        forceReadWrite = options.ForceReadWrite;
                    }
                    if (options.PropertyType != null)
                    {
                        property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, this);
                    }
                }
            }

            if (property == null)
            {
                property = CreateProperty();
            }

            Describe(property, descriptor);
            if (forceReadWrite)
            {
                property.IsReadOnly = false;
            }
            property.OnDescribed();
            property.RefreshValueFromDescriptor();
            return property;
        }

        protected virtual void ScanProperties()
        {
            Properties.Clear();
            List<PropertyGridProperty> props = new List<PropertyGridProperty>();
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Data))
            {
                if (!descriptor.IsBrowsable)
                    continue;

                PropertyGridProperty property = CreateProperty(descriptor);
                if (property != null)
                {
                    props.Add(property);
                }
            }

            IPropertyGridObject pga = Data as IPropertyGridObject;
            if (pga != null)
            {
                pga.FinalizeProperties(this, props);
            }

            props.Sort();
            foreach (PropertyGridProperty property in props)
            {
                Properties.Add(property);
            }
        }

        bool IListSource.ContainsListCollection
        {
            get { return false; }
        }

        IList IListSource.GetList()
        {
            return Properties;
        }
    }
}