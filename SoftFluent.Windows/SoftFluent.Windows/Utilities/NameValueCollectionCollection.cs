using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// Represents a collection of associated NameValueCollection objects, parsed from an input string, that can be accessed either with the key or with the index.
    /// </summary>
	internal class NameValueCollectionCollection: IEnumerable<NameValueCollection>
	{
        /// <summary>
        /// Defines the default separator character to use. Defined as ';'.
        /// </summary>
		public const char DefaultSeparator = ';';

        /// <summary>
        /// Defines the default quote character to use. Defined as '"'.
        /// </summary>
        public const char DefaultQuote = '"';

        /// <summary>
        /// Defines the default assignment character to use. Defined as '='.
        /// </summary>
        public const char DefaultAssignment = '=';

        private readonly List<NameValueCollection> _list = new List<NameValueCollection>();
		private readonly Dictionary<string, NameValueCollection> _table;
		private readonly Dictionary<object, string> _byValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollectionCollection"/> class.
        /// </summary>
		public NameValueCollectionCollection()
			:this(null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollectionCollection"/> class.
        /// </summary>
        /// <param name="text">The text to parse. May be null.</param>
		public NameValueCollectionCollection(string text)
			:this(text, DefaultSeparator, DefaultQuote, DefaultAssignment, true, false)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollectionCollection"/> class.
        /// </summary>
        /// <param name="text">The text to parse. May be null.</param>
        /// <param name="trim">if set to <c>true</c> names are trimmed.</param>
        /// <param name="caseSensitive">if set to <c>true</c> the created collection are case sensitive.</param>
        public NameValueCollectionCollection(string text, bool trim, bool caseSensitive)
            : this(text, DefaultSeparator, DefaultQuote, DefaultAssignment, trim, caseSensitive, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollectionCollection"/> class.
        /// </summary>
        /// <param name="text">The text to parse. May be null.</param>
        /// <param name="separator">The separator character to use.</param>
        /// <param name="quote">The quote character to use.</param>
        /// <param name="assignment">The assignment character to use.</param>
        /// <param name="trim">if set to <c>true</c> names are trimmed.</param>
        /// <param name="caseSensitive">if set to <c>true</c> the created collection are case sensitive.</param>
        public NameValueCollectionCollection(string text, char separator, char quote, char assignment, bool trim, bool caseSensitive)
			:this(text, separator, quote, assignment, trim, caseSensitive, false, false)
		{
		}

                /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollectionCollection"/> class.
        /// </summary>
        /// <param name="text">The text to parse. May be null.</param>
        /// <param name="separator">The separator character to use.</param>
        /// <param name="quote">The quote character to use.</param>
        /// <param name="assignment">The assignment character to use.</param>
        /// <param name="trim">if set to <c>true</c> names are trimmed.</param>
        /// <param name="caseSensitive">if set to <c>true</c> the created collection are case sensitive.</param>
        /// <param name="createByValueIndex">if set to <c>true</c> a by-value index is also created.</param>
        /// <param name="unescape">if set to <c>true</c> the input text is unescaped.</param>
        public NameValueCollectionCollection(
            string text,
            char separator,
            char quote,
            char assignment,
            bool trim,
            bool caseSensitive,
            bool createByValueIndex,
            bool unescape)
            :this(text, new[] { separator }, new[] { quote }, assignment, trim, caseSensitive, createByValueIndex, unescape, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollectionCollection"/> class.
        /// </summary>
        /// <param name="text">The text to parse. May be null.</param>
        /// <param name="separators">The separator characters to use.</param>
        /// <param name="quotes">The quote characters to use.</param>
        /// <param name="assignment">The assignment character to use.</param>
        /// <param name="trim">if set to <c>true</c> names are trimmed.</param>
        /// <param name="caseSensitive">if set to <c>true</c> the created collection are case sensitive.</param>
        /// <param name="createByValueIndex">if set to <c>true</c> a by-value index is also created.</param>
        /// <param name="unescape">if set to <c>true</c> the input text is unescaped.</param>
        /// <param name="keepQuotes">if set to <c>true</c> quotes are kept.</param>
		public NameValueCollectionCollection(
			string text,
			char[] separators,
			char[] quotes,
			char assignment,
			bool trim,
			bool caseSensitive,
			bool createByValueIndex,
			bool unescape,
            bool keepQuotes)
		{
			Text = text;
			SeparatorChars = separators;
			QuoteChars = quotes;
			AssignmentChar = assignment;
			MustTrim = trim;
            KeepQuotes = keepQuotes;
			MustUnescape = unescape;
			MustCreateByValueIndex = createByValueIndex;

			if (!caseSensitive)
			{
    			_table = new Dictionary<string,NameValueCollection>(StringComparer.CurrentCultureIgnoreCase);
				if (createByValueIndex)
				{
                    _byValue = new Dictionary<object, string>();
				}
			}
			else
			{
				_table = new Dictionary<string,NameValueCollection>();
			}
			Parse();
		}

        /// <summary>
        /// Gets a value indicating whether quotes are kept.
        /// </summary>
        /// <value><c>true</c> if quotes are kept; otherwise, <c>false</c>.</value>
        public bool KeepQuotes { get; private set; }

        /// <summary>
        /// Gets a value indicating whether names must be trimmed.
        /// </summary>
        /// <value><c>true</c> if names must be trimmed; otherwise, <c>false</c>.</value>
        public bool MustTrim { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a by-value index must be created.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if a by-value index must be created; otherwise, <c>false</c>.
        /// </value>
        public bool MustCreateByValueIndex { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the input text must be escaped.
        /// </summary>
        /// <value><c>true</c> if the input text must be escaped; otherwise, <c>false</c>.</value>
        public bool MustUnescape { get; private set; }

        /// <summary>
        /// Gets the first separator character.
        /// </summary>
        /// <value>The separator character.</value>
		public char SeparatorChar
		{
			get
			{
				return SeparatorChars[0];
			}
		}

        /// <summary>
        /// Gets the separator characters.
        /// </summary>
        /// <value>The separator characters.</value>
        public char[ ] SeparatorChars { get; private set; }

        /// <summary>
        /// Gets the quote character.
        /// </summary>
        /// <value>The quote character.</value>
		public char QuoteChar
		{
			get
			{
				return QuoteChars[0];
			}
		}

        /// <summary>
        /// Gets the quote characters.
        /// </summary>
        /// <value>The quote characters.</value>
        public char[ ] QuoteChars { get; private set; }

        /// <summary>
        /// Gets the assignment character.
        /// </summary>
        /// <value>The assignment character.</value>
        public char AssignmentChar { get; private set; }

        private void Parse()
		{
			string[] nameValues = Split(Text, SeparatorChars, QuoteChars, MustTrim, -1, KeepQuotes);
			for(int i = 0; i < nameValues.Length; i++)
			{
				string nameValue = nameValues[i];

                string[] split = Split(nameValue, new[] { AssignmentChar }, QuoteChars, MustTrim, 2);
				if (split.Length > 0 && split[0].Length > 0)
				{
					string s0 = split[0];
					if (MustUnescape)
					{
						s0 = Regex.Unescape(s0);
					}
					if (split.Length > 1)
					{
						string s1 = split[1];
						if (MustUnescape)
						{
							s1 = Regex.Unescape(s1);
						}
						Add(s0, s1);
					}
					else
					{
						Add(s0, null);
					}
				}
			}
		}

        /// <summary>
        /// Creates a string representation of a dictionary instance.
        /// </summary>
        /// <param name="dictionary">The input dictionary. May be null.</param>
        /// <returns>The output string</returns>
        public static string ToString(IDictionary dictionary)
        {
            return ToString(dictionary, DefaultSeparator, DefaultQuote, DefaultAssignment);
        }

        /// <summary>
        /// Creates a string representation of a dictionary instance.
        /// </summary>
        /// <param name="dictionary">The input dictionary. May be null.</param>
        /// <param name="separator">The separator character.</param>
        /// <param name="quote">The quote character.</param>
        /// <param name="assignment">The assignment character.</param>
        /// <returns>The output string</returns>
        public static string ToString(IDictionary dictionary, char separator, char quote, char assignment)
        {
            if (dictionary == null)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key == null)
                    continue;

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(entry.Key);
                sb.Append(assignment);
                if (entry.Value != null)
                {
                    bool mustQuote = (entry.Value.ToString().IndexOf(quote) >= 0);
                    if (mustQuote)
                    {
                        sb.Append(DefaultQuote);
                    }
                    sb.Append(entry.Value);
                    if (mustQuote)
                    {
                        sb.Append(DefaultQuote);
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a String array containing the substrings in this instance that are delimited by elements of a specified Char or String array.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <param name="separators">The separator characters.</param>
        /// <param name="quotes">The quote characters.</param>
        /// <param name="trim">if set to <c>true</c> the input names are trimmed.</param>
        /// <returns>
        /// An array whose elements contain the substrings in this instance that are delimited by one or more characters in separator.
        /// </returns>
        public static string[] Split(string text, char[] separators, char[] quotes, bool trim)
        {
            return Split(text, separators, quotes, trim, -1);
        }

        /// <summary>
        /// Returns a String array containing the substrings in this instance that are delimited by elements of a specified Char or String array.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <param name="separator">The separator character.</param>
        /// <param name="quote">The quote character.</param>
        /// <param name="trim">if set to <c>true</c> the input names are trimmed.</param>
        /// <returns>An array whose elements contain the substrings in this instance that are delimited by one or more characters in separator.</returns>
		public static string[] Split(string text, char separator, char quote, bool trim)
		{
			return Split(text, separator, quote, trim, -1);
		}

        private static bool IsIn(char c, char[] cs)
        {
            // we suppose separators & quotes are inherently case insensitive (=,'", etc...)
            for (int i = 0; i < cs.Length; i++)
            {
                if (c == cs[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a String array containing the substrings in this instance that are delimited by elements of a specified Char or String array.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <param name="separators">The separator characters.</param>
        /// <param name="quotes">The quote characters.</param>
        /// <param name="trim">if set to <c>true</c> the input names are trimmed.</param>
        /// <param name="maxCount">The maximum number of substrings to return.</param>
        /// <returns>
        /// An array whose elements contain the substrings in this instance that are delimited by one or more characters in separator.
        /// </returns>
        public static string[] Split(string text, char[] separators, char[] quotes, bool trim, int maxCount)
        {
            return Split(text, separators, quotes, trim, maxCount, false);
        }

        /// <summary>
        /// Returns a String array containing the substrings in this instance that are delimited by elements of a specified Char or String array.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <param name="separators">The separator characters.</param>
        /// <param name="quotes">The quote characters.</param>
        /// <param name="trim">if set to <c>true</c> the input names are trimmed.</param>
        /// <param name="maxCount">The maximum number of substrings to return.</param>
        /// <param name="keepQuotes">if set to <c>true</c> quotes are kept.</param>
        /// <returns>
        /// An array whose elements contain the substrings in this instance that are delimited by one or more characters in separator.
        /// </returns>
        public static string[] Split(string text, char[] separators, char[] quotes, bool trim, int maxCount, bool keepQuotes)
        {
            if (separators == null)
                throw new ArgumentNullException("separators");

            if (quotes == null)
                throw new ArgumentNullException("quotes");

            List<string> al = new List<string>();

            int i = 0;
            bool inQuote = false;
            StringBuilder chunk = new StringBuilder();

            char lastQuoteChar = '\0';
            while ((text != null) && (i < text.Length))
            {
                char c = text[i];
                if (inQuote)
                {
                    if (c == lastQuoteChar)
                    {
                        inQuote = false;
                        lastQuoteChar = '\0';
                        if (keepQuotes)
                        {
                            chunk.Append(c);
                        }
                    }
                    else
                    {
                        chunk.Append(c);
                    }
                }
                else
                {
                    if (IsIn(c, quotes))
                    {
                        lastQuoteChar = c;
                        inQuote = true;
                        if (keepQuotes)
                        {
                            chunk.Append(c);
                        }
                    }
                    else
                    {
                        if (IsIn(c, separators))
                        {
                            string s = chunk.ToString();
                            if (trim)
                            {
                                s = s.Trim();

                                if (s.Length > 0)
                                {
                                    al.Add(s);
                                }
                            }
                            else
                            {
                                al.Add(s);
                            }

                            if ((al.Count == (maxCount - 1)) && (maxCount > 0))
                            {
                                s = text.Substring(i + 1, text.Length - (i + 1));

                                if (trim)
                                {
                                    if (s.Length > 0)
                                    {
                                        al.Add(s);
                                    }
                                }
                                else
                                {
                                    al.Add(s);
                                }
                                break;
                            }

                            chunk.Length = 0;
                        }
                        else
                        {
                            chunk.Append(c);
                        }
                    }
                }
                i++;
            }

            if ((al.Count != (maxCount - 1)) || (maxCount <= 0))
            {
                string s = chunk.ToString();

                if (trim)
                {
                    s = s.Trim();
                    if (s.Length > 0)
                    {
                        al.Add(s);
                    }
                }
                else
                {
                    al.Add(s);
                }
            }
            return al.ToArray();
        }

        /// <summary>
        /// Returns a String array containing the substrings in this instance that are delimited by elements of a specified Char or String array.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <param name="separator">The separator character.</param>
        /// <param name="quote">The quote character.</param>
        /// <param name="trim">if set to <c>true</c> the input names are trimmed.</param>
        /// <param name="maxCount">The maximum number of substrings to return.</param>
        /// <returns>
        /// An array whose elements contain the substrings in this instance that are delimited by one or more characters in separator.
        /// </returns>
        public static string[] Split(string text, char separator, char quote, bool trim, int maxCount)
		{
            return Split(text, new[] { separator }, new[] { quote }, trim, maxCount);
		}

        /// <summary>
        /// Gets the original input text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the <see cref="NameValueCollection"/> at the specified index.
        /// </summary>
        /// <value>The collection instance.</value>
		public NameValueCollection this[int index]
		{
			get
			{
				return _list[index];
			}
		}

        /// <summary>
        /// Adds the specified value to the collection.
        /// </summary>
        /// <param name="name">The name to use. may not be null.</param>
        /// <param name="value">The value.</param>
		public void Add(string name, object value)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if (list == null)
			{
				list = new NameValueCollection(name);
				Add(list);
			}
			list.Add(value);
			if (_byValue != null)
			{
				_byValue[value] = list.Name;
			}
		}

        /// <summary>
        /// Adds the specified list to the collection.
        /// </summary>
        /// <param name="list">The input list. May not be null.</param>
		public void Add(NameValueCollection list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			_list.Add(list);
			_table[list.Name] = list;
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public object GetValue(string name, int index, object defaultValue)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			return list[index];
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public string GetValue(string name, int index, string defaultValue)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			return o.ToString();
		}

        /// <summary>
        /// Gets the first value at the specified index.
        /// </summary>
        /// <typeparam name="T">The value target type.</typeparam>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public T GetValue<T>(string name, T defaultValue)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            NameValueCollection list = this[name];
            if ((list == null) || (list.Count == 0))
                return defaultValue;

            return ConvertUtilities.ChangeType(list[0], defaultValue);
        }

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <typeparam name="T">The value target type.</typeparam>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public T GetValue<T>(string name, int index, T defaultValue)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            NameValueCollection list = this[name];
            if ((list == null) || (list.Count <= index))
                return defaultValue;

            object o = list[index];
            if (o == null)
                return defaultValue;

            return ConvertUtilities.ChangeType(o, defaultValue);
        }

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public bool GetValue(string name, int index, bool defaultValue)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return bool.Parse(o.ToString());
			}
			catch(FormatException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public int GetValue(string name, int index, int defaultValue, IFormatProvider provider)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return int.Parse(o.ToString(), provider);
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
			catch(FormatException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public byte GetValue(string name, int index, byte defaultValue, IFormatProvider provider)
        {
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return byte.Parse(o.ToString(), provider);
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
			catch(FormatException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public short GetValue(string name, int index, short defaultValue, IFormatProvider provider)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return short.Parse(o.ToString(), provider);
			}
			catch(FormatException)
			{
				return defaultValue;
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public long GetValue(string name, int index, long defaultValue, IFormatProvider provider)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return long.Parse(o.ToString(), provider);
			}
			catch(FormatException)
			{
				return defaultValue;
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public TimeSpan GetValue(string name, int index, TimeSpan defaultValue)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return TimeSpan.Parse(o.ToString());
			}
			catch(FormatException)
			{
				return defaultValue;
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public DateTime GetValue(string name, int index, DateTime defaultValue, IFormatProvider provider)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return DateTime.Parse(o.ToString(), provider);
			}
			catch(FormatException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public Guid GetValue(string name, int index, Guid defaultValue)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return new Guid(o.ToString());
			}
			catch(FormatException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public decimal GetValue(string name, int index, decimal defaultValue, IFormatProvider provider)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return decimal.Parse(o.ToString(), provider);
			}
			catch(FormatException)
			{
				return defaultValue;
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public double GetValue(string name, int index, double defaultValue, IFormatProvider provider)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return double.Parse(o.ToString(), provider);
			}
			catch(FormatException)
			{
				return defaultValue;
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <param name="provider">The input provider format provider.</param>
        /// <returns>The value at the specified index.</returns>
        public float GetValue(string name, int index, float defaultValue, IFormatProvider provider)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

			try
			{
				return float.Parse(o.ToString(), provider);
			}
			catch(FormatException)
			{
				return defaultValue;
			}
			catch(OverflowException)
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
        public char GetValue(string name, int index, char defaultValue)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

            char c;
            if (!char.TryParse(o.ToString(), out c))
                return defaultValue;

            return c;
		}

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="name">The value name. May not be null.</param>
        /// <param name="index">The index to search at.</param>
        /// <param name="defaultValue">The default value if an error occurs.</param>
        /// <returns>The value at the specified index.</returns>
		public Enum GetValue(string name, int index, Enum defaultValue)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (defaultValue == null)
				throw new ArgumentNullException("defaultValue");

			NameValueCollection list = this[name];
			if ((list == null) || (list.Count <= index))
				return defaultValue;

			object o = list[index];
			if (o == null)
				return defaultValue;

            return ConvertUtilities.ToEnum(o.ToString(), defaultValue);
		}

        /// <summary>
        /// Gets the <see cref="NameValueCollection"/> with the specified name.
        /// </summary>
        /// <value>The collection instance.</value>
		public NameValueCollection this[string name]
		{
			get
			{
				if (name == null)
					throw new ArgumentNullException("name");

                NameValueCollection nvc;
                _table.TryGetValue(name, out nvc);
                return nvc;
			}
		}

        /// <summary>
        /// Gets a name by its value.
        /// Only available if the by-value index has been created.
        /// if multiple names have the same value, the results are not deterministic.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The name.</returns>
		public string GetNameByValue(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (_byValue == null)
				throw new InvalidOperationException();

            string s;
            _byValue.TryGetValue(value, out s);
            return s;
		}

#if !NETFX_CORE
        /// <summary>
        /// Converts to a dictionary.
        /// </summary>
        /// <returns>An instance of a Hashtable type.</returns>
        public Hashtable ToTable()
        {
            return (Hashtable)ToDictionary(false);
        }

        /// <summary>
        /// Converts to a dictionary.
        /// </summary>
        /// <returns>An instance of a IDictionary type.</returns>
        public OrderedDictionary ToDictionary()
        {
            return (OrderedDictionary)ToDictionary(true);
        }
#endif

        /// <summary>
        /// Converts to a dictionary.
        /// </summary>
        /// <typeparam name="T">The type of target values.</typeparam>
        /// <returns>An instance of a IDictionary type.</returns>
        public IDictionary<string, T> ToDictionary<T>()
        {
            Dictionary<string, T> table = new Dictionary<string, T>(StringComparer.CurrentCultureIgnoreCase);
            foreach (NameValueCollection c in _list)
            {
                if (_list.Count == 0)
                {
                    table[c.Name] = default(T);
                }
                else
                {
                    // add last
                    table[c.Name] = ConvertUtilities.ChangeType<T>(c[c.Count - 1]);
                }
            }
            return table;
        }

        /// <summary>
        /// Converts to a dictionary.
        /// </summary>
        /// <param name="ordered">if set to <c>true</c>, the target IDictionary will be ordered.</param>
        /// <returns>An instance of a IDictionary type.</returns>
        public IDictionary ToDictionary(bool ordered)
        {
            IDictionary table;
#if NETFX_CORE
            if (ordered)
                throw new ArgumentException(null, "ordered");

            table = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
#else
            if (ordered)
            {
                table = new OrderedDictionary(StringComparer.CurrentCultureIgnoreCase);
            }
            else
            {
                table = new Hashtable(StringComparer.CurrentCultureIgnoreCase);
            }
#endif
            foreach (NameValueCollection c in _list)
            {
                if (_list.Count == 0)
                {
                    table[c.Name] = null;
                }
                else
                {
                    // add last
                    table[c.Name] = c[c.Count - 1];
                }
            }
            return table;
        }

        /// <summary>
        /// Gets the count of collections in this collection.
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

        IEnumerator<NameValueCollection> IEnumerable<NameValueCollection>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
