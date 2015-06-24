using System;

namespace SoftFluent.Windows
{
    public interface ITypeResolver
    {
        Type ResolveType(string fullName, bool throwOnError);
    }
}
