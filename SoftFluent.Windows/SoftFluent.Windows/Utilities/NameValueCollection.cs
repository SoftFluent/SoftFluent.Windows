using System.Collections;
using System.Collections.Generic;

namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// Defines a collection of values for the same key name.
    /// </summary>
    internal class NameValueCollection : IEnumerable
	{
        private readonly List<object> _list = new List<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
		public NameValueCollection(string name)
		{
			Name = name;
		}

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the first value or null.
        /// </summary>
        /// <value>The first value or null.</value>
        public object FirstValue
        {
            get
            {
                return _list.Count == 0 ? null : _list[0];
            }
        }

        /// <summary>
        /// Adds the specified value to the collection.
        /// </summary>
        /// <param name="value">The value to add.</param>
		public void Add(object value)
		{
			_list.Add(value);
		}

        /// <summary>
        /// Gets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value>The object instance.</value>
		public object this[int index]
		{
			get
			{
				return _list[index];
			}
		}

        /// <summary>
        /// Gets the count of values.
        /// </summary>
        /// <value>The count.</value>
		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}
	}
}
