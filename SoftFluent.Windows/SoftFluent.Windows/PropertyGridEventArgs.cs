using System.ComponentModel;

namespace SoftFluent.Windows
{
    public class PropertyGridEventArgs : CancelEventArgs
    {
        public PropertyGridEventArgs(PropertyGridProperty property)
            : this(property, null)
        {
        }

        public PropertyGridEventArgs(PropertyGridProperty property, object context)
        {
            Property = property;
            Context = context;
        }

        public PropertyGridProperty Property { get; private set; }
        public object Context { get; set; }
    }
}