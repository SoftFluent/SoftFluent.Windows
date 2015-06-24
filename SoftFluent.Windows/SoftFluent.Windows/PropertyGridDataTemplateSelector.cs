using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{

    [ContentProperty("DataTemplates")]
    public class PropertyGridDataTemplateSelector : DataTemplateSelector
    {
        private PropertyGrid _propertyGrid;

        public PropertyGridDataTemplateSelector()
        {
            DataTemplates = new ObservableCollection<PropertyGridDataTemplate>();
        }

        public PropertyGrid PropertyGrid
        {
            get
            {
                return _propertyGrid;
            }
        }

        protected virtual bool Filter(PropertyGridDataTemplate template, PropertyGridProperty property)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            if (property == null)
                throw new ArgumentNullException("property");

            // check various filters
            if (template.IsCollection.HasValue && template.IsCollection.Value != property.IsCollection)
            {
                return true;
            }

            if (template.IsCollectionItemValueType.HasValue && template.IsCollectionItemValueType.Value != property.IsCollectionItemValueType)
            {
                return true;
            }

            if (template.IsValueType.HasValue && template.IsValueType.Value != property.IsValueType)
            {
                return true;
            }

            if (template.IsReadOnly.HasValue && template.IsReadOnly.Value != property.IsReadOnly)
            {
                return true;
            }

            if (template.IsError.HasValue && template.IsError.Value != property.IsError)
            {
                return true;
            }

            if (template.IsValid.HasValue && template.IsValid.Value != property.IsValid)
            {
                return true;
            }

            if (template.IsFlagsEnum.HasValue && template.IsFlagsEnum.Value != property.IsFlagsEnum)
            {
                return true;
            }

            if (template.Category != null && !property.Category.EqualsIgnoreCase(template.Category))
            {
                return true;
            }

            if (template.Name != null && !property.Name.EqualsIgnoreCase(template.Name))
            {
                return true;
            }

            return false;
        }

        public virtual bool IsAssignableFrom(Type type1, Type type2)
        {
            if (type1.IsAssignableFrom(type2))
            {
                // bool? is assignable from bool, but we don't want that match
                if (!type1.IsNullable() || type2.IsNullable())
                    return true;
            }

            // hack for nullable enums...
            if (type1 == PropertyGridDataTemplate.NullableEnumType)
            {
                Type enumType;
                bool nullable;
                PropertyGridProperty.IsExtendedEnum(type2, out enumType, out nullable);
                if (nullable)
                    return true;
            }

            return false;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PropertyGridProperty property = item as PropertyGridProperty;
            if (property == null)
                return base.SelectTemplate(item, container);

            DataTemplate propTemplate = PropertyGridOptionsAttribute.SelectTemplate(property, item, container);
            if (propTemplate != null)
                return propTemplate;

            if (_propertyGrid == null)
            {
                _propertyGrid = container.GetVisualSelfOrParent<PropertyGrid>();
            }

            if (_propertyGrid.ValueEditorTemplateSelector != null && _propertyGrid.ValueEditorTemplateSelector != this)
            {
                DataTemplate template = _propertyGrid.ValueEditorTemplateSelector.SelectTemplate(item, container);
                if (template != null)
                    return template;
            }

            foreach (PropertyGridDataTemplate template in DataTemplates)
            {
                if (Filter(template, property))
                    continue;

                if (template.IsCollection.HasValue && template.IsCollection.Value)
                {
                    if (string.IsNullOrWhiteSpace(template.CollectionItemPropertyType) && template.DataTemplate != null)
                        return template.DataTemplate;

                    if (property.CollectionItemPropertyType != null)
                    {
                        foreach (Type type in template.ResolvedCollectionItemPropertyTypes)
                        {
                            if (IsAssignableFrom(type, property.CollectionItemPropertyType))
                                return template.DataTemplate;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(template.PropertyType) && template.DataTemplate != null)
                        return template.DataTemplate;

                    foreach (Type type in template.ResolvedPropertyTypes)
                    {
                        if (IsAssignableFrom(type, property.PropertyType))
                            return template.DataTemplate;
                    }
                }
            }
            return base.SelectTemplate(item, container);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<PropertyGridDataTemplate> DataTemplates { get; private set; }
    }
}
