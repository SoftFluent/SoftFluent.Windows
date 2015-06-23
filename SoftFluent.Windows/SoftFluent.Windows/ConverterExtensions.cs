using System;

namespace SoftFluent.Windows
{
    public static class ConverterExtensions
    {
        public static bool TryChangeType<T>(this IConverter converter, object input, IFormatProvider provider, out T value)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");

            object v;
            if (!converter.TryChangeType(input, typeof(T), provider, out v))
            {
                value = default(T);
                return false;
            }

            value = (T)v;
            return true;
        }
    }
}
