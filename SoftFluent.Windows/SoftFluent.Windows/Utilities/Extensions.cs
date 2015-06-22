using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoftFluent.Windows.Utilities
{
    internal static class Extensions
    {
        public static string GetAllMessages(this Exception exception)
        {
            return GetAllMessages(exception, Environment.NewLine);
        }

        public static string GetAllMessages(this Exception exception, string separator)
        {
            if (exception == null)
                return null;

            StringBuilder sb = new StringBuilder();
            AppendMessages(sb, exception, separator);
            return sb.ToString().Replace("..", ".");
        }

        private static void AppendMessages(StringBuilder sb, Exception e, string separator)
        {
            if (e == null)
                return;

            // this one is not interesting...
            if (!(e is TargetInvocationException))
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(e.Message);
            }
            AppendMessages(sb, e.InnerException, separator);
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            if (provider == null)
                return null;

            object[] o = provider.GetCustomAttributes(typeof(T), true);
            if (o == null || o.Length == 0)
                return null;

            return (T)o[0];
        }

        public static T GetAttribute<T>(this MemberDescriptor descriptor) where T : Attribute
        {
            if (descriptor == null)
                return null;

            return GetAttribute<T>(descriptor.Attributes);
        }

        public static T GetAttribute<T>(this AttributeCollection attributes) where T : Attribute
        {
            if (attributes == null)
                return null;

            foreach (Attribute att in attributes)
            {
                if (typeof(T).IsAssignableFrom(att.GetType()))
                    return (T)att;
            }
            return null;
        }
    }
}
