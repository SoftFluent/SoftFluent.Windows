using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace SoftFluent.Windows.Resources
{
	sealed internal class SR
	{
		private static SR _loader;
		private readonly ResourceManager _resources;

		private SR()
		{
#if NETFX_CORE
            _resources = new ResourceManager("CodeFluent.Runtime.Resources.Strings", GetType().GetTypeInfo().Module.Assembly);
#else
            _resources = new ResourceManager("CodeFluent.Runtime.Resources.Strings", GetType().Module.Assembly);
#endif
        }

        private static SR GetLoader()
		{
			if (_loader == null)
			{
				Type type;

				Monitor.Enter(type = typeof(SR));
				try
				{
					if (_loader == null)
					{
						_loader = new SR();
					}
				}
				finally
				{
					Monitor.Exit(type);
				}
			}
			return _loader;
		}

		public static string GetString(string name, params object[] args)
		{
			return GetString(null, name, args);
		}

		public static string GetString(string name, object arg0)
		{
			return GetString(null, name, new object[]{arg0});
		}

		public static string GetString(string name, object arg0, object arg1)
		{
			return GetString(null, name, new object[]{arg0, arg1});
		}

		public static string GetString(string name, object arg0, object arg1, object arg2)
		{
			return GetString(null, name, new object[]{arg0, arg1, arg2});
		}
		
		public static string GetString(string name, object arg0, object arg1, object arg2, object arg3)
		{
			return GetString(null, name, new object[]{arg0, arg1, arg2, arg3});
		}

		public static string GetString(CultureInfo culture, string name, object arg0)
		{
			return GetString(culture, name, new object[]{arg0});
		}

		public static string GetString(CultureInfo culture, string name, object arg0, object arg1)
		{
			return GetString(culture, name, new object[]{arg0, arg1});
		}

		public static string GetString(CultureInfo culture, string name, object arg0, object arg1, object arg2)
		{
			return GetString(culture, name, new object[]{arg0, arg1, arg2});
		}
		
		public static string GetString(CultureInfo culture, string name, object arg0, object arg1, object arg2, object arg3)
		{
			return GetString(culture, name, new object[]{arg0, arg1, arg2, arg3});
		}
		
        public static string GetString(CultureInfo culture, string name, params object[] args)
        {
            SR loader = GetLoader();
            if (loader == null)
                return null;

            string str = loader._resources.GetString(name, culture);
            if (string.IsNullOrEmpty(str))
                return name;

            if (args != null && args.Length > 0)
                return string.Format(culture, str, args);

            return str;
        }

        public static string GetString(string name)
        {
            return GetString(null, name);
        }

        public static string GetString(CultureInfo culture, string name)
        {
            SR loader = GetLoader();
            if (loader == null)
                return null;

            string str = loader._resources.GetString(name, culture);
            if (string.IsNullOrEmpty(str))
                return name;

            return str;
        }
	}
}
