using System;

namespace SoftFluent.Windows
{
    public interface IActivator
    {
        object CreateInstance(Type type, params object[] args);
    }
}
