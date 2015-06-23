using System.Reflection;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridEnumProperty : PropertyGridProperty
    {
        public PropertyGridEnumProperty(PropertyGridDataProvider provider)
            : base(provider)
        {
            EnumAttributes = new DynamicObject();
        }

        public override void OnDescribed()
        {
        }

        public virtual DynamicObject EnumAttributes { get; private set; }

        public override object Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
                EnumAttributes.Properties.Clear();
                foreach (FieldInfo fi in PropertyType.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (fi.Name.Equals(string.Format("{0}", base.Value)))
                    {
                        PropertyGridDataProvider.AddDynamicProperties(fi.GetAttributes<PropertyGridAttribute>(), EnumAttributes);
                    }
                }
            }
        }
    }
}