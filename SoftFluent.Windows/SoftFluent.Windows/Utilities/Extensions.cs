using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SoftFluent.Windows.Utilities
{
    internal static class Extensions
    {
        public static bool EqualsIgnoreCase(this string thisString, string text)
        {
            return EqualsIgnoreCase(thisString, text, false);
        }

        public static bool EqualsIgnoreCase(this string thisString, string text, bool trim)
        {
            if (trim)
            {
                thisString = ConvertUtilities.Nullify(thisString, true);
                text = ConvertUtilities.Nullify(text, true);
            }

            if (thisString == null)
                return text == null;

            if (text == null)
                return false;

            if (thisString.Length != text.Length)
                return false;

            return string.Compare(thisString, text, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj)
        {
            return obj.EnumerateVisualChildren(true);
        }

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj, bool recursive)
        {
            return obj.EnumerateVisualChildren(recursive, true);
        }

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj, bool recursive, bool sameLevelFirst)
        {
            if (obj == null)
                yield break;

            if (sameLevelFirst)
            {
                int count = VisualTreeHelper.GetChildrenCount(obj);
                List<DependencyObject> list = new List<DependencyObject>(count);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child == null)
                        continue;

                    yield return child;
                    if (recursive)
                    {
                        list.Add(child);
                    }
                }

                foreach (var child in list)
                {
                    foreach (DependencyObject grandChild in child.EnumerateVisualChildren(recursive, sameLevelFirst))
                    {
                        yield return grandChild;
                    }
                }
            }
            else
            {
                int count = VisualTreeHelper.GetChildrenCount(obj);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child == null)
                        continue;

                    yield return child;
                    if (recursive)
                    {
                        foreach (var dp in child.EnumerateVisualChildren(recursive, sameLevelFirst))
                        {
                            yield return dp;
                        }
                    }
                }
            }
        }

        public static T FindVisualChild<T>(this DependencyObject obj, Func<T, bool> where) where T : FrameworkElement
        {
            if (where == null)
                throw new ArgumentNullException("where");

            foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
            {
                if (where(item))
                    return item;
            }
            return null;
        }

        public static T FindVisualChild<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
            {
                if (name == null)
                    return item;

                if (item.Name == name)
                    return item;
            }
            return null;
        }

        public static IEnumerable<DependencyProperty> EnumerateMarkupDependencyProperties(object element)
        {
            if (element != null)
            {
                MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
                if (markupObject != null)
                {
                    foreach (MarkupProperty mp in markupObject.Properties)
                    {
                        if (mp.DependencyProperty != null)
                            yield return mp.DependencyProperty;
                    }
                }
            }
        }

        public static IEnumerable<DependencyProperty> EnumerateMarkupAttachedProperties(object element)
        {
            if (element != null)
            {
                MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
                if (markupObject != null)
                {
                    foreach (MarkupProperty mp in markupObject.Properties)
                    {
                        if (mp.IsAttached)
                            yield return mp.DependencyProperty;
                    }
                }
            }
        }

        public static T GetVisualSelfOrParent<T>(this DependencyObject source) where T : DependencyObject
        {
            if (source == null)
                return default(T);

            if (source is T)
                return (T)source;

            if (!(source is Visual) && !(source is Visual3D))
                return default(T);

            return VisualTreeHelper.GetParent(source).GetVisualSelfOrParent<T>();
        }
        public static T FindFocusableVisualChild<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
            {
                if (item.Focusable && (item.Name == name || name == null))
                    return item;
            }
            return null;
        }

        public static IEnumerable<T> GetChildren<T>(this DependencyObject obj)
        {
            if (obj == null)
                yield break;

            foreach (object item in LogicalTreeHelper.GetChildren(obj))
            {
                if (item == null)
                    continue;

                if (item is T)
                    yield return (T)item;

                DependencyObject dep = item as DependencyObject;
                if (dep != null)
                {
                    foreach (T child in dep.GetChildren<T>())
                    {
                        yield return child;
                    }
                }
            }
        }

        public static T GetSelfOrParent<T>(this FrameworkElement source) where T : FrameworkElement
        {
            while (true)
            {
                if (source == null)
                    return default(T);

                if (source is T)
                    return (T)source;

                source = source.Parent as FrameworkElement;
            }
        }

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

        public static IEnumerable<T> GetAttributes<T>(this MemberInfo element) where T : Attribute
        {
            return (IEnumerable<T>)Attribute.GetCustomAttributes(element, typeof(T));
        }
    }
}
