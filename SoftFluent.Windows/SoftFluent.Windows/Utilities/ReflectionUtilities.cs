using System;

namespace SoftFluent.Windows.Utilities
{
    internal static class ReflectionUtilities
    {
        public static Type GetType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            Type type = Type.GetType(typeName, false);
            return type;
        }
    }
}
