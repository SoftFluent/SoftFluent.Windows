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
            _resources = new ResourceManager(typeof(SR).Namespace + ".Strings", GetType().Module.Assembly);
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
