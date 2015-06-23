using System;
using System.Collections.Concurrent;

namespace SoftFluent.Windows
{
    public class ServiceProvider : IServiceProvider
    {
        private ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();
        private static readonly ServiceProvider _current = new ServiceProvider();

        public ServiceProvider()
        {
            ResetDefaultServices();
        }

        protected virtual void ResetDefaultServices()
        {
            if (!_services.ContainsKey(typeof(IConverter)))
            {
                _services[typeof(IConverter)] = new BaseConverter();
            }
        }

        public static ServiceProvider Current
        {
            get
            {
                return _current;
            }
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public virtual object SetService(Type serviceType, object service)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            // service can be null, it will reset to the default one

            object previous;
            _services.TryGetValue(serviceType, out previous);
            _services[serviceType] = service;
            ResetDefaultServices();
            return previous;
        }

        public virtual object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            object value;
            _services.TryGetValue(serviceType, out value);
            return value;
        }

        public static bool TryChangeType<T>(object input, IFormatProvider provider, out T value)
        {
            return Current.GetService<IConverter>().TryChangeType(input, provider, out value);
        }

        public static bool TryChangeType<T>(object input, out T value)
        {
            return TryChangeType(input, null, out value);
        }

        public static bool TryChangeType(object input, Type conversionType, out object value)
        {
            return TryChangeType(input, conversionType, null, out value);
        }

        public static bool TryChangeType(object input, Type conversionType, IFormatProvider provider, out object value)
        {
            return Current.GetService<IConverter>().TryChangeType(input, conversionType, provider, out value);
        }

        public static object ChangeType(object input, Type conversionType)
        {
            return ChangeType(input, conversionType, null, (IFormatProvider)null);
        }

        public static object ChangeType(object input, Type conversionType, object defaultValue)
        {
            return ChangeType(input, conversionType, defaultValue, (IFormatProvider)null);
        }

        public static object ChangeType(object input, Type conversionType, object defaultValue, IFormatProvider provider)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (defaultValue == null && conversionType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(conversionType);
            }

            object value;
            if (TryChangeType(input, conversionType, provider, out value))
                return value;

            return defaultValue;
        }

        public static T ChangeType<T>(object input)
        {
            return ChangeType(input, default(T));
        }

        public static T ChangeType<T>(object input, T defaultValue)
        {
            return ChangeType(input, null, defaultValue);
        }

        public static T ChangeType<T>(object input, IFormatProvider provider, T defaultValue)
        {
            T value;
            if (TryChangeType(input, provider, out value))
                return value;

            return defaultValue;
        }
    }
}
