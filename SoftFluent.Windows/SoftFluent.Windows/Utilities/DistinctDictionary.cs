using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if NETFX_CORE
using System.Reflection;
#endif

namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// Provides a class for a collection whose keys are the same as values.
    /// It is used for computing distinct values from a collection.
    /// </summary>
    /// <typeparam name="T">The value and key type</typeparam>
    internal class DistinctDictionary<T>: KeyedCollection<T, T>, IDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctDictionary&lt;T&gt;"/> class.
        /// </summary>
        public DistinctDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctDictionary&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">An existing collection.</param>
        public DistinctDictionary(IEnumerable<T> collection)
        {
            Construct(collection);
        }

        private void Construct(IEnumerable<T> collection)
        {
            if (collection != null)
            {
                foreach (T t in collection)
                {
                    Add(t);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctDictionary&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from <see cref="P:System.Collections.Generic.EqualityComparer`1.Default"/>.</param>
        public DistinctDictionary(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctDictionary&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from <see cref="P:System.Collections.Generic.EqualityComparer`1.Default"/>.</param>
        /// <param name="collection">An existing collection.</param>
        public DistinctDictionary(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(comparer)
        {
            Construct(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctDictionary&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from <see cref="P:System.Collections.Generic.EqualityComparer`1.Default"/>.</param>
        /// <param name="dictionaryCreationThreshold">The number of elements the collection can hold without creating a lookup dictionary (0 creates the lookup dictionary when the first item is added), or –1 to specify that a lookup dictionary is never created.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="dictionaryCreationThreshold"/> is less than –1.</exception>
        public DistinctDictionary(IEqualityComparer<T> comparer, int dictionaryCreationThreshold)
            :base(comparer, dictionaryCreationThreshold)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctDictionary&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">An existing collection.</param>
        /// <param name="comparer">The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from <see cref="P:System.Collections.Generic.EqualityComparer`1.Default"/>.</param>
        /// <param name="dictionaryCreationThreshold">The number of elements the collection can hold without creating a lookup dictionary (0 creates the lookup dictionary when the first item is added), or –1 to specify that a lookup dictionary is never created.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="dictionaryCreationThreshold"/> is less than –1.</exception>
        public DistinctDictionary(IEnumerable<T> collection, IEqualityComparer<T> comparer, int dictionaryCreationThreshold)
            : base(comparer, dictionaryCreationThreshold)
        {
            Construct(collection);
        }

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override T GetKeyForItem(T item)
        {
            return item;
        }

        /// <summary>
        /// Adds the elements of the specified collection.
        /// </summary>
        /// <param name="collection">The collection of elements to add. May not be null.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (T t in collection)
            {
                if (t == null)
                    continue;

                Add(t);
            }
        }

        /// <summary>
        /// Removes the elements from the specified collection.
        /// </summary>
        /// <param name="collection">The collection of elements to remove. May not be null.</param>
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (T t in collection)
            {
                if (t == null)
                    continue;

                Remove(t);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        /// <exception cref="System.ArgumentNullException">item</exception>
        protected override void InsertItem(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (Contains(item))
                return;

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Adds an element to the end of the Collection if the element was not already added.
        /// </summary>
        /// <param name="item">The object to be added to the end of the collection.</param>
        public new void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (Contains(item))
                return;

            base.Add(item);
        }

        /// <summary>
        /// Adds an element to the end of the Collection if the element was not already added.
        /// </summary>
        /// <param name="item">The object to be added to the end of the collection.</param>
        public void AddOrReplace(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Remove(item);
            base.Add(item);
        }

        /// <summary>
        /// Copies the elements of the dictionary to a new list.
        /// </summary>
        /// <returns>A list of elements in the dictionary.</returns>
        public List<T> ToList()
        {
            return new List<T>(this);
        }

        /// <summary>
        /// Copies the elements of the dictionary to a new array.
        /// </summary>
        /// <returns>An array of elements in the dictionary.</returns>
        public T[] ToArray()
        {
            T[] array = new T[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = this[i];
            }
            return array;
        }

        /// <summary>
        /// Gets the distinct values for a given collection.
        /// </summary>
        /// <param name="collection">An existing collection. May not be null.</param>
        /// <returns>The list of distinct values.</returns>
        public static IEnumerable<T> GetDistinctValues(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            DistinctDictionary<T> dict = new DistinctDictionary<T>();
            foreach (T t in collection)
            {
                if (!dict.Contains(t))
                {
                    dict.Add(t);
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <param name="key">The <see cref="T:System.Object"/> to use as the key of the element to add.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// An element with the same key already exists in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IDictionary"/> is read-only.
        /// -or-
        /// The <see cref="T:System.Collections.IDictionary"/> has a fixed size.
        /// </exception>
        void IDictionary.Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

#if NETFX_CORE
            if (!typeof(T).GetTypeInfo().IsAssignableFrom(key.GetType().GetTypeInfo()))
                throw new ArgumentException(null, "key");
#else
            if (!typeof(T).IsAssignableFrom(key.GetType()))
                throw new ArgumentException(null, "key");
#endif

            Add((T)key);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IDictionary"/> object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"/> object.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IDictionary"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        bool IDictionary.Contains(object key)
        {
            if (key == null)
                return false;

#if NETFX_CORE
            if (!typeof(T).GetTypeInfo().IsAssignableFrom(key.GetType().GetTypeInfo()))
                return false;
#else
            if (!typeof(T).IsAssignableFrom(key.GetType()))
                return false;
#endif

            return Contains((T)key);
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Dicenum(GetEnumerator());
        }

        private class Dicenum : IDictionaryEnumerator
        {
            private IEnumerator<T> _enumerator;

            public Dicenum(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    return new DictionaryEntry(_enumerator.Current, _enumerator.Current);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    return _enumerator.Current;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    return _enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return new DictionaryEntry(_enumerator.Current, _enumerator.Current);
                }
            }

            bool IEnumerator.MoveNext()
            {
                return _enumerator.MoveNext();
            }

            void IEnumerator.Reset()
            {
                _enumerator.Reset();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
        /// </returns>
        bool IDictionary.IsFixedSize
        {
            get
            {
                return ((IList)this).IsFixedSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool IDictionary.IsReadOnly
        {
            get
            {
                return ((IList)this).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        ICollection IDictionary.Keys
        {
            get
            {
                Collection<T> coll = new Collection<T>();
                foreach (T item in this)
                {
                    coll.Add(item);
                }
                return coll;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        ICollection IDictionary.Values
        {
            get
            {
                return ((IDictionary)this).Keys;
            }
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IDictionary"/> object is read-only.
        /// -or-
        /// The <see cref="T:System.Collections.IDictionary"/> has a fixed size.
        /// </exception>
        void IDictionary.Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

#if NETFX_CORE
            if (!typeof(T).GetTypeInfo().IsAssignableFrom(key.GetType().GetTypeInfo()))
                throw new ArgumentException(null, "key");
#else
            if (!typeof(T).IsAssignableFrom(key.GetType()))
                throw new ArgumentException(null, "key");
#endif

            Remove((T)key);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        object IDictionary.this[object key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

#if NETFX_CORE
                if (!typeof(T).GetTypeInfo().IsAssignableFrom(key.GetType().GetTypeInfo()))
                    throw new ArgumentException(null, "key");
#else
                if (!typeof(T).IsAssignableFrom(key.GetType()))
                    throw new ArgumentException(null, "key");
#endif

                return Contains((T)key) ? key : null;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

#if NETFX_CORE
                if (!typeof(T).GetTypeInfo().IsAssignableFrom(key.GetType().GetTypeInfo()))
                    throw new ArgumentException(null, "key");
#else
                if (!typeof(T).IsAssignableFrom(key.GetType()))
                    throw new ArgumentException(null, "key");
#endif

                Add((T)key);
            }
        }
    }
}
