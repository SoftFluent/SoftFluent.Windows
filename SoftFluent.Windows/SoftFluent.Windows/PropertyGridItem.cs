using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridItem : AutoObject
    {
        public PropertyGridItem()
        {
            IsChecked = false;
        }

        public virtual string Name { get { return GetProperty<string>(); } set { SetProperty<string>(value); } }
        public virtual object Value { get { return GetProperty<object>(); } set { SetProperty<object>(value); } }
        public virtual bool? IsChecked { get { return GetProperty<bool?>(); } set { SetProperty<bool?>(value); } }
        public virtual PropertyGridProperty Property { get { return GetProperty<PropertyGridProperty>(); } set { SetProperty<PropertyGridProperty>(value); } }

        public override string ToString()
        {
            return Name;
        }
    }
}