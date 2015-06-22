#if !SILVERLIGHT && !NETFX_CORE
#if !CLIENT_PROFILE
#endif
#endif

#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Server;
using SoftFluent.Windows.Resources;

namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// A set of conversion utilities.
    /// </summary>
	internal sealed class ConvertUtilities
	{
        /// <summary>
        /// Defines what is the default value for System.Boolean values transferred accross persistence layers.
        /// Currently defined as 'false'.
        /// </summary>
        public const bool DefaultBooleanValue = false;

        /// <summary>
        /// Defines what is the default value for System.Char values transferred accross persistence layers.
        /// Currently defined as '\0'.
        /// </summary>
        public const char DefaultCharValue = '\0';

        /// <summary>
        /// Defines what is the default value for System.Byte values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        public const byte DefaultByteValue = 0;

        /// <summary>
        /// Defines what is the default value for System.Decimal values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        public const decimal DefaultDecimalValue = 0;

        /// <summary>
        /// Defines what is the default value for System.Double values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        public const double DefaultDoubleValue = 0;

        /// <summary>
        /// Defines what is the default value for System.Int16 values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        public const short DefaultInt16Value = 0;

        /// <summary>
        /// Defines what is the default value for System.Int32 values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        public const int DefaultInt32Value = 0;

        /// <summary>
        /// Defines what is the default value for System.Int64 values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        public const long DefaultInt64Value = 0;

        /// <summary>
        /// Defines what is the default value for System.SByte values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        [CLSCompliant(false)]
        public const sbyte DefaultSByteValue = 0;

        /// <summary>
        /// Defines what is the default value for System.Single values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        public const float DefaultSingleValue = 0;

        /// <summary>
        /// Defines what is the default value for System.UInt16 values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        [CLSCompliant(false)]
        public const ushort DefaultUInt16Value = 0;

        /// <summary>
        /// Defines what is the default value for System.UInt32 values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        [CLSCompliant(false)]
        public const uint DefaultUInt32Value = 0;

        /// <summary>
        /// Defines what is the default value for System.UInt64 values transferred accross persistence layers.
        /// Currently defined as 0.
        /// </summary>
        [CLSCompliant(false)]
        public const ulong DefaultUInt64Value = 0;

        /// <summary>
        /// Defines what is the default value for System.Guid values transferred accross persistence layers.
        /// Currently defined as Guid.Empty.
        /// </summary>
        public readonly static Guid DefaultGuidValue = Guid.Empty;

        /// <summary>
        /// Defines what is the default value for System.DateTime values transferred accross persistence layers.
        /// Currently defined as DateTime.MinValue.
        /// </summary>
        public readonly static DateTime DefaultDateTimeValue = DateTime.MinValue;
        
        private const string _hexaChars = "0123456789ABCDEF";
        private static char[] _enumSeperators = new char[] { ',', ';', '+', '|', ' ' };

		private ConvertUtilities()
		{
		}
        
        /// <summary>
        /// Concatenates a collection into a string using a separator character.
        /// An expression is ran on each object in the collection using ASP.NET DataBinding syntax style.
        /// </summary>
        /// <param name="collection">The input collection. If null, null will be returned.</param>
        /// <param name="expression">The input expression. May be null.</param>
        /// <param name="separator">The separator character.</param>
        /// <param name="formatProvider">The format provider to use. May be null.</param>
        /// <returns>A string.</returns>
        public static string ConcatenateCollection(IEnumerable collection, string expression, string separator, IFormatProvider formatProvider)
        {
            if (collection == null)
                return null;

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (object o in collection)
            {
                if (i > 0)
                {
                    sb.Append(separator);
                }
                else
                {
                    i++;
                }
                if (o != null)
                {
                    object e = Evaluate(o, expression, typeof(string), null, formatProvider);
                    if (e != null)
                    {
                        sb.Append(e);
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Concatenates the specified objects using their string representation.
        /// </summary>
        /// <param name="objects">The collection of objects to concatenate. If the enumerable is null, null will be returned.</param>
        /// <param name="separator">The separator character.</param>
        /// <returns>A string.</returns>
		public static string Concatenate(IEnumerable objects, string separator)
		{
            if (objects == null)
				return null;

			StringBuilder sb = new StringBuilder();
			int i = 0;
            foreach (object obj in objects)
			{
				if (i > 0)
				{
					sb.Append(separator);
				}
				else
				{
					i++;
				}
                if (obj != null)
				{
                    sb.Append(obj);
				}
			}
			return sb.ToString();
		}
     
        /// <summary>
        /// Splits a text into a collection of typed objects.
        /// Returned objects are automatically converted into the proper type if needed.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="text">The text to split. If null, an empty list will be returned.</param>
        /// <param name="separators">The separator characters to use.</param>
        /// <returns>A collection of objects of the T type.</returns>
        public static List<T> SplitToList<T>(string text, params char[] separators)
        {
            List<T> al = new List<T>();
            if (text == null)
                return al;

            foreach (string s in text.Split(separators))
            {
                string value = s.Trim();
                if (value.Length == 0)
                    continue;

                T item = (T)ChangeType(value, typeof(T), default(T), null);
                al.Add(item);
            }
            return al;
        }

        /// <summary>
        /// Converts a byte array to its hexadecimal string representation.
        /// </summary>
        /// <param name="bytes">The input byte array. If null, an empty string will be returned.</param>
        /// <returns>The string representation of the byte array.</returns>
		public static string ToHexa(byte[] bytes)
		{
            if (bytes == null)
                return String.Empty;

            return ToHexa(bytes, 0, bytes.Length);
		}

        /// <summary>
        /// Converts a byte array to its hexadecimal string representation.
        /// </summary>
        /// <param name="bytes">The input byte array. If null, an empty string will be returned.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin dumping bytes.</param>
        /// <param name="count">The number of bytes to be dumped.</param>
        /// <returns>
        /// The string representation of the byte array.
        /// </returns>
        public static string ToHexa(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return String.Empty;

            if (offset < 0)
                throw new ArgumentException(null, "offset");

            if (count < 0)
                throw new ArgumentException(null, "count");

            if (offset >= bytes.Length)
                return String.Empty;

            count = Math.Min(count, bytes.Length - offset);

            StringBuilder sb = new StringBuilder(count * 2);
            for (int i = offset; i < (offset + count); i++)
            {
                sb.Append(_hexaChars[bytes[i] / 16]);
                sb.Append(_hexaChars[bytes[i] % 16]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a string representation of a byte array to a byte array.
        /// Invalid characters are just skipped, without throwing any exception.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <param name="separators">A list of possible separators. Mut contain at least one element.</param>
        /// <returns>The converted byte array. May be null.</returns>
        public static byte[] ToBytesFromArray(string text, params char[] separators)
        {
            return ToBytesFromArray(text, false, separators);
        }

        /// <summary>
        /// Converts a string representation of a byte array to a byte array.
        /// Invalid characters are just skipped, without throwing any exception.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <param name="throwOnError">if set to <c>true</c> errors may be throw if the input text is invalid.</param>
        /// <param name="separators">A list of possible separators. Mut contain at least one element.</param>
        /// <returns>The converted byte array. May be null.</returns>
        public static byte[] ToBytesFromArray(string text, bool throwOnError, params char[] separators)
        {
            if ((separators == null) || (separators.Length == 0))
                throw new ArgumentNullException("separators");

            if (text == null)
                return null;

            List<byte> list = new List<byte>();
            int b = 0;
            int radix = 10;
            for (int i = 0; i < text.Length; i++)
            {
                if (Match(text[i], separators))
                {
                    if (b < 256)
                    {
                        list.Add((byte)b);
                    }
                    b = 0;
                    radix = 10;
                }
                else if ((radix == 10) && (Char.IsDigit(text[i])))
                {
                    b = radix * b + text[i] - '0';
                    if (b > 255)
                    {
                        if (throwOnError)
                            throw new ArgumentException("text");
                    }
                }
                else if ((radix == 16) && (GetHexaByte(text[i]) != 0xFF))
                {
                    b = radix * b + GetHexaByte(text[i]);
                    if (b > 255)
                    {
                        if (throwOnError)
                            throw new ArgumentException("text");
                    }
                }
                else if (((text[i] == 'x') || (text[i] == 'X')) && (b == 0))
                {
                    radix = 16;
                }
                else
                {
                    if (throwOnError)
                        throw new ArgumentException("text");

                    b = 256;
                }
            }
            if (b < 256)
            {
                list.Add((byte)b);
            }
            return list.ToArray();
        }

        private static bool Match(char c, char[] separators)
        {
            foreach (char separator in separators)
            {
                if (c == separator)
                    return true;
            }
            return false;
        }

        // this also works for guids as strings, because we skip wrong chars
        /// <summary>
        /// Converts an hexadecimal string representation to a byte array.
        /// Invalid characters are just skipped, without throwing any exception.
        /// This function also supports <see cref="System.Guid"/> string representation.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <returns>The converted byte array. May be null.</returns>
        public static byte[] ToBytesFromHexa(string text)
		{
            if (text == null)
                return null;

			List<byte> list = new List<byte>();
			bool lo = false;
            byte prev = 0;
			int offset;

			// handle 0x or 0X notation
			if ((text.Length >= 2) && (text[0] == '0') && ((text[1] == 'x') || (text[1] == 'X')))
			{
				offset = 2;
			}
			else
			{
				offset = 0;
			}
			for(int i = 0; i < text.Length - offset; i++)
			{
				byte b = GetHexaByte(text[i + offset]);
				if (b == 0xFF)
					continue;

				if (lo)
				{
					list.Add((byte)(prev * 16 + b));
				}
				else
				{
					prev = b;
				}
				lo = !lo;
			}

			return list.ToArray();
		}

        /// <summary>
        /// Gets the hexadecimal byte for the specified character.
        /// </summary>
        /// <param name="c">The input character.</param>
        /// <returns>The corresponding hexadecimal byte.</returns>
		public static byte GetHexaByte(char c)
		{
			if ((c >= '0') && (c <= '9'))
				return (byte)(c - '0');

            if ((c >= 'A') && (c <= 'F'))
				return (byte)(c - 'A' + 10);

            if ((c >= 'a') && (c <= 'f'))
				return (byte)(c - 'a' + 10);

            return 0xFF;
		}
		
		/// <summary>
        /// Evaluates the specified expression on a given container object.
        /// In case of an error, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="container">The container object. If null, the defaultValue parameter will be returned.</param>
        /// <param name="expression">The DataBinder.Eval expression. May not be null.</param>
        /// <param name="conversionType">The target type. May not be null.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A value of the target type.</returns>
        public static object Evaluate(object container, string expression, Type conversionType, object defaultValue, IFormatProvider provider)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            object o;
            if (container == null)
            {
                o = ChangeType(defaultValue, conversionType);
            }
            else
            {
                if (expression == null)
                {
                    o = ChangeType(container, conversionType, defaultValue, provider);
                }
                else
                {

                    try
                    {
                        o = DataBindingEvaluator.Eval(container, expression);
                        if (o == null)
                        {
                            o = ChangeType(defaultValue, conversionType);
                        }
                        else
                        {
                            o = ChangeType(o, conversionType, defaultValue, provider);
                        }
                    }
                    catch
                    {
                        o = ChangeType(defaultValue, conversionType);
                    }
                }
            }
            return o;
        }

        /// <summary>
        /// Evaluates the specified expression on a given container object.
        /// In case of an error, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="container">The container object. If null, the defaultValue parameter will be returned.</param>
        /// <param name="expression">The DataBinder.Eval expression. May not be null.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A value of the target type.</returns>
        public static object Evaluate(object container, string expression, object defaultValue)
		{
			if (container == null)
				return defaultValue;

			try
			{
				return DataBindingEvaluator.Eval(container, expression);
	        }
			catch
			{
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets a CLR type from a full type name.
        /// This function supports CodeFluent type aliases.
        /// </summary>
        /// <param name="fullTypeName">The full type name. May not be null.</param>
        /// <returns>A CLR type instance.</returns>
		public static Type GetType(string fullTypeName)
		{
			if (fullTypeName == null)
				throw new ArgumentNullException("fullTypeName");

            bool nullable = false;
            if (fullTypeName.EndsWith("?"))
            {
                nullable = true;
                fullTypeName = fullTypeName.Substring(0, fullTypeName.Length - 1);
            }

#if NETFX_CORE
            switch (fullTypeName.ToLower())
#else
			switch(fullTypeName.ToLower(CultureInfo.CurrentCulture))
#endif
			{
				case "bit":
				case "bool":
				case "boolean":
				case "system.boolean":
                    if (nullable)
                        return typeof(Nullable<bool>);
					
                    return typeof(bool);

				case "char":
				case "system.char":
                    if (nullable)
                        return typeof(Nullable<char>);
                    
                    return typeof(char);

				case "byte":
				case "system.byte":
                    if (nullable)
                        return typeof(Nullable<byte>);
                   
                    return typeof(byte);

				case "xml":
                case "email":
                case "mail":
                case "text":
                case "richtext":
                case "password":
				case "url":
				case "hyperlink":
				case "string":
                case "richstring":
                case "system.string":
					return typeof(string);

				case "time":
				case "date":
				case "datetime":
                case "datetime2":
                case "system.datetime":
                    if (nullable)
                        return typeof(Nullable<DateTime>);
                    
                    return typeof(DateTime);

                case "duration":
                case "elapsedtime":
				case "timespan":
                case "system.timespan":
                    if (nullable)
                        return typeof(Nullable<TimeSpan>);
                    
                    return typeof(TimeSpan);

                case "system.datetimeoffset":
                case "datetimeoffset":
                    if (nullable)
                        return typeof(Nullable<DateTimeOffset>);

                    return typeof(DateTimeOffset);

                case "culture":
                case "cultureinfo":
                case "system.globalization.cultureinfo":
                    return typeof(CultureInfo);

                case "cost":
                case "price":
                case "money":
				case "currency":
				case "decimal":
				case "system.decimal":
                    if (nullable)
                        return typeof(Nullable<decimal>);
                    
                    return typeof(decimal);

				case "real":
				case "double":
				case "system.double":
                    if (nullable)
                        return typeof(Nullable<double>);
                    
                    return typeof(double);

				case "short":
				case "int16":
				case "system.int16":
                    if (nullable)
                        return typeof(Nullable<short>);
                   
                    return typeof(short);

				case "int32":
				case "int":
				case "system.int32":
                    if (nullable)
                        return typeof(Nullable<int>);
                    
                    return typeof(int);

				case "int64":
				case "long":
				case "system.int64":
                    if (nullable)
                        return typeof(Nullable<long>);
                    
                    return typeof(long);

				case "sbyte":
				case "system.sbyte":
                    if (nullable)
                        return typeof(Nullable<sbyte>);
                    
                    return typeof(sbyte);

				case "single":
				case "float":
				case "system.single":
                    if (nullable)
                        return typeof(Nullable<float>);
                    
                    return typeof(float);

				case "ushort":
				case "uint16":
				case "system.uint16":
                    if (nullable)
                        return typeof(Nullable<ushort>);
                    
                    return typeof(ushort);

				case "uint":
				case "uint32":
				case "system.uint32":
                    if (nullable)
                        return typeof(Nullable<uint>);
                    
                    return typeof(uint);

				case "ulong":
				case "uint64":
				case "system.uint64":
                    if (nullable)
                        return typeof(Nullable<ulong>);
                    
                    return typeof(ulong);

				case "uuid":
				case "uniqueidentifier":
				case "guid":
				case "system.guid":
                    if (nullable)
                        return typeof(Nullable<Guid>);
                    
                    return typeof(Guid);

				case "attachment":
                case "video":
                case "audio":
                case "sound":
                case "document":
                case "picture":
                case "film":
                case "movie":
                case "file":
				case "photo":
                case "largebinary":
                case "image":
				case "blob":
				case "binary":
				case "data":
				case "byte[]":
                case "codefluent.runtime.binaryservices.binarylargeobject":
				case "system.byte[]":
					return typeof(byte[]);

				case "anyvalue":
				case "object":
				case "any":
				case "system.object":
					return typeof(object);

#if !SILVERLIGHT && !NETFX_CORE
                case "system.xml.xmldocument":
                    return typeof(XmlDocument);
#endif

				default:
#if SILVERLIGHT
                    Type type = Type.GetType(fullTypeName, false, true);
#else
#if NETFX_CORE
                    Type type = Type.GetType(fullTypeName, false);
#else
                    Type type = AssemblyUtilities.GetType(fullTypeName, false);
#endif
#endif
#if NETFX_CORE
                    if ((nullable) && (type != null) && (type.IsValueType()))
#else
                    if ((nullable) && (type != null) && (type.IsValueType))
#endif
                        return typeof(Nullable<>).MakeGenericType(type);

                    return type;
			}
        }

        /// <summary>
        /// Gets the type of elements in a collection type.
        /// </summary>
        /// <param name="collectionType">The collection type.</param>
        /// <returns>The element type or typeof(object) if it was not determined.</returns>
        public static Type GetItemType(Type collectionType)
        {
            if (collectionType == null)
                throw new ArgumentNullException("collectionType");

            foreach (Type iface in collectionType.GetInterfaces())
            {
#if NETFX_CORE
                if (!iface.IsGenericType())
#else
                if (!iface.IsGenericType)
#endif
                    continue;

                if (iface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return iface.GetGenericArguments()[1];

                if (iface.GetGenericTypeDefinition() == typeof(IList<>))
                    return iface.GetGenericArguments()[0];

                if (iface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return iface.GetGenericArguments()[0];

                if (iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return iface.GetGenericArguments()[0];
            }
            return typeof(object);
        }

        /// <summary>
        /// Returns a System.Object with a specified type and whose value is equivalent to a specified input object.
        /// If an error occurs, a computed default value of the target type will be returned.
        /// </summary>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="fullTypeName">The target full type name. May not be null.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static object ChangeType(object value, string fullTypeName, IFormatProvider provider)
		{
			if (fullTypeName == null)
				throw new ArgumentNullException("fullTypeName");

			Type type = GetType(fullTypeName);
			if (type == null)
				throw new ArgumentException(SR.GetString("unhandledConversionType2", fullTypeName), "fullTypeName");

			return ChangeType(value, type, provider);
		}

#if !SILVERLIGHT && !NETFX_CORE

        /// <summary>
        /// Converts a DbType into a CLR type.
        /// </summary>
        /// <param name="dbType">The input DbType.</param>
        /// <returns>An instance of the Type class.</returns>
        public static Type ToType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Binary:
                    return typeof(byte[]);

                case DbType.Boolean:
                    return typeof(bool);

                case DbType.Byte:
                    return typeof(byte);

                case DbType.Decimal:
                case DbType.Currency:
                case DbType.VarNumeric:
                    return typeof(decimal);
                
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.Time:
                    return typeof(DateTime);

                case DbType.DateTimeOffset:
                    return typeof(DateTimeOffset);
                
                case DbType.Double:
                    return typeof(double);
                
                case DbType.Guid:
                    return typeof(Guid);
                
                case DbType.Int16:
                    return typeof(short);
                
                case DbType.Int32:
                    return typeof(int);
                
                case DbType.Int64:
                    return typeof(long);

                case DbType.Object:
                    return typeof(object);
                
                case DbType.SByte:
                    return typeof(sbyte);
                
                case DbType.Single:
                    return typeof(float);
                
                case DbType.UInt16:
                    return typeof(ushort);

                case DbType.UInt32:
                    return typeof(uint);

                case DbType.UInt64:
                    return typeof(ulong);

                //case DbType.AnsiString:
                //case DbType.AnsiStringFixedLength:
                //case DbType.String:
                //case DbType.Xml:
                //case DbType.StringFixedLength:
                default:
                    return typeof(string);
            }
        }
#endif

        /// <summary>
        /// Returns a System.Object with a specified type and whose value is equivalent to a specified input object.
        /// If an error occurs, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="conversionType">The target type. May not be null.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static object ChangeType(object value, Type conversionType, object defaultValue)
		{
			return ChangeType(value, conversionType, defaultValue, null);
		}
   
        /// <summary>
        /// Determines whether the specified character is a valid identifier start.
        /// Valids unicode categories are Lu, Ll, Lt, Lm, Lo, Nl
        /// </summary>
        /// <param name="character">The input character.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is a valid identifier start; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidIdentifierStart(char character)
        {
            if (character == '_')
                return true;

            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);
            switch (category)
            {
                case UnicodeCategory.UppercaseLetter://Lu
                case UnicodeCategory.LowercaseLetter://Ll
                case UnicodeCategory.TitlecaseLetter://Lt
                case UnicodeCategory.ModifierLetter://Lm
                case UnicodeCategory.OtherLetter://Lo
                case UnicodeCategory.LetterNumber://Nl
                    return true;

                default:
                    return false;
            }
        }


        /// <summary>
        /// Determines whether the specified character is a valid identifier part.
        /// Valids unicode categories are Lu, Ll, Lt, Lm, Lo, Nl, Mn, Mc, Nd, Pc, Cf
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is a valid identifier part; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidIdentifierPart(char character)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);
            switch (category)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.LetterNumber:

                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:

                case UnicodeCategory.DecimalDigitNumber:

                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.Format:
                    return true;

                default:
                    return false;
            }
        }

		private static string UnscrambleFlagsValue(string flagsEnumValue)
		{
			// mostly used for excel's multivalue notation
			if (flagsEnumValue == null)
				return null;

			StringBuilder sb = new StringBuilder(flagsEnumValue.Length);
			bool inSeparator = false;
			for(int i = 0; i < flagsEnumValue.Length; i++)
			{
				char c = flagsEnumValue[i];

				if (inSeparator)
				{
					if ((IsValidIdentifierPart(c)) ||
						(IsValidIdentifierStart(c)))
					{
						if (sb.Length > 0)
						{
							sb.Append(',');
						}
						inSeparator = false;
						sb.Append(c);
					}
					// else go on
				}
				else
				{
					if ((IsValidIdentifierPart(c)) ||
						(IsValidIdentifierStart(c)))
					{
						sb.Append(c);
					}
					else
					{
						inSeparator = true;
					}
				}
			}
			return sb.ToString();
		}

        /// <summary>
        /// Returns an instance of type T whose value is equivalent to a specified input object.
        /// If an error occurs, a computed default value of type T will be returned.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The input object. May be null.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(value, typeof(T), ChangeType(null, typeof(T)), null);
        }

        /// <summary>
        /// Returns an instance of type T whose value is equivalent to a specified input object.
        /// If an error occurs, a computed default value of type T will be returned.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static T ChangeType<T>(object value, IFormatProvider provider)
        {
            return (T)ChangeType(value, typeof(T), ChangeType(null, typeof(T)), provider);
        }

        /// <summary>
        /// Returns an instance of type T whose value is equivalent to a specified input object.
        /// If an error occurs, the defaultValue parameter will be returned.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static T ChangeType<T>(object value, T defaultValue)
        {
            return (T)ChangeType(value, typeof(T), defaultValue, null);
        }

        /// <summary>
        /// Returns an instance of type T whose value is equivalent to a specified input object.
        /// If an error occurs, the defaultValue parameter will be returned.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static T ChangeType<T>(object value, T defaultValue, IFormatProvider provider)
        {
            return (T)ChangeType(value, typeof(T), defaultValue, provider);
        }

#if !NETFX_CORE
        internal static bool IsChangeTypeConverter(TypeConverter cvt)
        {
            if (cvt == null)
                return false;

            if (!cvt.GetType().IsGenericType)
                return false;

            return (cvt.GetType().GetGenericTypeDefinition() == typeof(ChangeTypeConverter<>));
        }
#endif

        private static bool CanChangeType(Type conversionType)
        {
#if NETFX_CORE
            TypeCode code = conversionType.GetTypeCode();
#else
            TypeCode code = Type.GetTypeCode(conversionType);
#endif
            return ((code != TypeCode.DBNull) && (code != TypeCode.Empty) && (code != TypeCode.Object));
        }

        /// <summary>
        /// Returns a System.Object with a specified type and whose value is equivalent to a specified input object.
        /// If an error occurs, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="conversionType">The target type. May not be null.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static object ChangeType(object value, Type conversionType, object defaultValue, IFormatProvider provider)
		{
			if (conversionType == null)
				throw new ArgumentNullException("conversionType");

#if !NETFX_CORE
            if (Convert.IsDBNull(value))
            {
                value = null;
            }
#endif

            // TODO: implement To InstanceDescriptor general conversion
            if ((value != null) && (conversionType.IsAssignableFrom(value.GetType())))
                return value;

#if NETFX_CORE
            if ((value == null) && (conversionType.IsGenericType()) && (conversionType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return defaultValue;
#else
            if ((value == null) && (conversionType.IsGenericType) && (conversionType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return defaultValue;
#endif

            object typedDefaultValue = ChangeType(defaultValue, conversionType, provider);

#if NETFX_CORE
			if (conversionType.IsEnum())
#else
			if (conversionType.IsEnum)
#endif
			{
				string s;
				if (defaultValue == null)
				{
					s = (string)ChangeType(value, typeof(string), provider);
				}
				else
				{
					s = (string)ChangeType(value, typeof(string), defaultValue.ToString(), provider);
				}

                if (conversionType.IsDefined(typeof(FlagsAttribute), true))
				{
					s = UnscrambleFlagsValue(s);
				}

#if !SILVERLIGHT
                object enumValue;
                if (EnumTryParse(conversionType, s, out enumValue))
                    return enumValue;

                return typedDefaultValue;
#else
                s = Nullify(s, true);
                if (s == null)
                    return typedDefaultValue;

                try
                {
                    return Enum.Parse(conversionType, s, true);
                }
                catch
                {
                    return typedDefaultValue;
                }
#endif
			}

			if (conversionType == typeof(byte[]))
			{
				if (value == null)
					return null;

				if (value is Guid)
					return ((Guid)value).ToByteArray();
				

#if !SILVERLIGHT && !NETFX_CORE
                IBinarySerialize bs = value as IBinarySerialize; // sql geometry, geography, hierarchyid
                if (bs != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            bs.Write(writer);
                            return stream.ToArray();
                        }
                    }
                }
#endif

                if (!(value is string))
                {
                    IEnumerable en = value as IEnumerable;
                    if (en != null)
                    {
                        List<byte> bytes = new List<byte>();
                        foreach (object obj in en)
                        {
                            bytes.Add(ChangeType<byte>(obj, CultureInfo.InvariantCulture));
                        }
                        return bytes.ToArray();
                    }
                }

                if (value.ToString().IndexOf(',') > 0)
                    return ToBytesFromArray(value.ToString(), ',');

				return ToBytesFromHexa(value.ToString());
			}

			if (conversionType == typeof(Guid))
			{
				if (value == null)
					return typedDefaultValue;

				if (value is byte[])
				{
					try
					{
						return new Guid((byte[])value);
					}
					catch
					{
						return typedDefaultValue;
					}
				}

                return ToGuid(value.ToString(), (Guid)typedDefaultValue);
			}

#if !SILVERLIGHT
            if (conversionType == typeof(IntPtr))
            {
                if (IntPtr.Size == 4)
                    return new IntPtr((int)ChangeType(value, typeof(int), defaultValue, provider));

                if (IntPtr.Size == 8)
                    return new IntPtr((long)ChangeType(value, typeof(long), defaultValue, provider));
            }

            if (value is IntPtr)
            {
                if (IntPtr.Size == 4)
                    return ChangeType(((IntPtr)value).ToInt32(), conversionType, 0, provider);

                if (IntPtr.Size == 8)
                    return ChangeType(((IntPtr)value).ToInt64(), conversionType, 0L, provider);
            }

#if !NETFX_CORE
            if (conversionType == typeof(XmlDocument))
                return ToXmlDocument(value, typedDefaultValue as XmlDocument);
#endif
#endif
            if (conversionType == typeof(CultureInfo))
            {
                CultureInfo defCulture = (defaultValue == null) ? null : typedDefaultValue as CultureInfo;
                if (value == null)
#if SILVERLIGHT
                    return defCulture;
#else
                    return ToCultureInfo(null, false, defCulture);
#endif
#if SILVERLIGHT
                return ToCultureInfo(value.ToString());
#else
                return ToCultureInfo(value.ToString(), false, defCulture);
#endif
            }

			if (conversionType == typeof(TimeSpan))
			{
				if (value == null)
					return typedDefaultValue;

                if (value is long)
                    return new TimeSpan((long)value);
                
                TimeSpan ts;
				if (TimeSpan.TryParse(value.ToString(), out ts))
                    return ts;

                return typedDefaultValue;
			}

#if NETFX_CORE
            TypeCode typeCode = conversionType.GetTypeCode();
#else
			TypeCode typeCode = Type.GetTypeCode(conversionType);
#endif
			switch(typeCode)
			{
                case TypeCode.Boolean:
                    return ToBoolean(value, (bool)typedDefaultValue);

                case TypeCode.Decimal:
                case TypeCode.String:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Object:
					if ((typeCode == TypeCode.String) && (value is byte[]))
						return ToHexa((byte[])value);

                    if ((typeCode == TypeCode.UInt16) && (value is short))
                    {
                        return unchecked((ushort)(short)value);
                    }
                    else if ((typeCode == TypeCode.UInt32) && (value is int))
                    {
                        return unchecked((uint)(int)value);
                    }
                    else if ((typeCode == TypeCode.UInt64) && (value is long))
                    {
                        return unchecked((ulong)(long)value);
                    }
                    else if ((typeCode == TypeCode.Byte) && (value is sbyte))
                    {
                        return unchecked((byte)(sbyte)value);
                    }
                    else if ((typeCode == TypeCode.Int16) && (value is ushort))
                    {
                        return unchecked((short)(ushort)value);
                    }
                    else if ((typeCode == TypeCode.Int32) && (value is uint))
                    {
                        return unchecked((int)(uint)value);
                    }
                    else if ((typeCode == TypeCode.Int64) && (value is ulong))
                    {
                        return unchecked((long)(ulong)value);
                    }
                    else if ((typeCode == TypeCode.SByte) && (value is byte))
                    {
                        return unchecked((sbyte)(byte)value);
                    }

                    if ((typeCode == TypeCode.Decimal) && (value is string))
                    {
                        NumberFormatInfo nfi = provider as NumberFormatInfo;
                        if (nfi == null)
                        {
                            nfi = CultureInfo.CurrentCulture.NumberFormat;
                        }
                        value = ((string)value).Replace(nfi.CurrencySymbol, "");
                    }

#if NETFX_CORE
                    if ((conversionType.IsGenericType()) && (!conversionType.IsGenericTypeDefinition()))
#else
                    if ((conversionType.IsGenericType) && (!conversionType.IsGenericTypeDefinition))
#endif
                    {
                        if (conversionType.Name == typeof(Nullable<>).Name)
                        {
                            Type nt = typeof(Nullable<>).MakeGenericType(conversionType.GetGenericArguments()[0]);
                            if (String.Empty.Equals(value))
                                return Activator.CreateInstance(nt);

                            object i = Activator.CreateInstance(nt, ChangeType(value, conversionType.GetGenericArguments()[0], defaultValue, provider));
                            return i;
                        }
                    }

#if !NETFX_CORE
					IConvertible convertible = value as IConvertible;
					if (CanChangeType(conversionType) && (convertible != null))
					{
                        if ((!String.Empty.Equals(value)) || (!conversionType.IsValueType))
                        {
                            try
                            {
                                return Convert.ChangeType(value, conversionType, provider);
                            }
                            catch
                            {
                                // continue
                            }
                        }
					}
#endif

                    if (conversionType == typeof(string))
					{
						if (value == null)
							return typedDefaultValue;
					}

                    if ((conversionType == typeof(long)) && ((value != null) && (value.GetType() == typeof(TimeSpan))))
                        return ((TimeSpan)value).Ticks;
                    
					if (value == null)
						return typedDefaultValue;

#if !SILVERLIGHT && !NETFX_CORE
                    try
                    {
                        TypeConverter cvt = TypeDescriptor.GetConverter(value);
                        if ((cvt != null) && (!IsChangeTypeConverter(cvt)) && (cvt.CanConvertTo(conversionType)))
                            return cvt.ConvertTo(null, provider as CultureInfo, value, conversionType);

                        cvt = TypeDescriptor.GetConverter(conversionType);
                        if ((cvt != null) && (cvt.CanConvertFrom(value.GetType())))
                            return cvt.ConvertFrom(null, provider as CultureInfo, value);
                        
                    }
                    catch
                    {
                        return typedDefaultValue;
                    }
#endif

                    object outValue;
                    // don't use provider here because TryParse does not support it
                    if (TryParse(String.Format("{0}", value), conversionType, provider, out outValue))
                        return outValue;

                    //if (conversionType.IsValueType)
                    //    return conversionType.Assembly.CreateInstance(conversionType.FullName);
                    
                    return typedDefaultValue;

				default:
					if (value == null)
						return typedDefaultValue;

					throw new ArgumentException(SR.GetString("unhandledConversionType", conversionType.FullName, value.ToString(), value.GetType().FullName));
			}
		}

        /// <summary>
        /// Returns a System.Object with a specified type and whose value is equivalent to a specified input object.
        /// If an error occurs, a computed default value of the target type will be returned.
        /// </summary>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="conversionType">The target type. May not be null.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static object ChangeType(object value, Type conversionType)
		{
			return ChangeType(value, conversionType, null);
		}

        /// <summary>
        /// Returns a System.Object with a specified type and whose value is equivalent to a specified input object.
        /// If an error occurs, a computed default value of the target type will be returned.
        /// </summary>
        /// <param name="value">The input object. May be null.</param>
        /// <param name="conversionType">The target type. May not be null.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An object of the target type whose value is equivalent to input value.
        /// </returns>
        public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
		{
			if (conversionType == null)
				throw new ArgumentNullException("conversionType");

#if !NETFX_CORE
            if (Convert.IsDBNull(value))
            {
                value = null;
            }
#endif

            if ((value != null) && (conversionType.IsAssignableFrom(value.GetType())))
                return value;

#if NETFX_CORE
            if ((value == null) && (conversionType.IsGenericType()) && (conversionType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return null;

            if (conversionType.IsEnum())
#else
            if ((value == null) && (conversionType.IsGenericType) && (conversionType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return null;

			if (conversionType.IsEnum)
#endif
			{
				string s = (string)ChangeType(value, typeof(string), provider);
				if (conversionType.IsDefined(typeof(FlagsAttribute), true))
				{
					s = UnscrambleFlagsValue(s);
				}

#if !SILVERLIGHT
                object enumValue;
                EnumTryParse(conversionType, s, out enumValue);
                return enumValue;
#else
                if (string.IsNullOrEmpty(s))
                    return conversionType.Assembly.CreateInstance(conversionType.FullName);
                
                try
                {
                    return Enum.Parse(conversionType, s, true);
                }
                catch
                {
                    return conversionType.Assembly.CreateInstance(conversionType.FullName);
                }
#endif
            }

			if (conversionType == typeof(byte[]))
			{
				if (value == null)
					return null;

				if (value is Guid)
					return ((Guid)value).ToByteArray();
				

#if !SILVERLIGHT && !NETFX_CORE
                IBinarySerialize bs = value as IBinarySerialize; // sql geometry, geography, hierarchyid
                if (bs != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            bs.Write(writer);
                            return stream.ToArray();
                        }
                    }
                }
#endif

                if (!(value is string))
                {
                    IEnumerable en = value as IEnumerable;
                    if (en != null)
                    {
                        List<byte> bytes = new List<byte>();
                        foreach (object obj in en)
                        {
                            bytes.Add(ChangeType<byte>(obj, CultureInfo.InvariantCulture));
                        }
                        return bytes.ToArray();
                    }
                }
                
                if (value.ToString().IndexOf(',') > 0)
                    return ToBytesFromArray(value.ToString(), ',');

                return ToBytesFromHexa(value.ToString());
			}

			if (conversionType == typeof(Guid))
			{
				if (value == null)
					return DefaultGuidValue;

				if (value is byte[])
				{
					try
					{
						return new Guid((byte[])value);
					}
					catch
					{
						return DefaultGuidValue;
					}
				}
                return ToGuid(value.ToString(), DefaultGuidValue);
			}

#if !SILVERLIGHT
            if (conversionType == typeof(IntPtr))
            {
                if (IntPtr.Size == 4)
                    return new IntPtr((int)ChangeType(value, typeof(int), provider));

                if (IntPtr.Size == 8)
                    return new IntPtr((long)ChangeType(value, typeof(long), provider));
            }

            if (value is IntPtr)
            {
                if (IntPtr.Size == 4)
                    return ChangeType(((IntPtr)value).ToInt32(), conversionType, provider);

                if (IntPtr.Size == 8)
                    return ChangeType(((IntPtr)value).ToInt64(), conversionType, provider);
            }

#if !NETFX_CORE
            if (conversionType == typeof(XmlDocument))
                return ToXmlDocument(value, null);
#endif
#endif

            if (conversionType == typeof(CultureInfo))
            {
                if (value == null)
                    return null;
                   //return ToCultureInfo(null);
                
                return ToCultureInfo(value.ToString());
            }

			if (conversionType == typeof(TimeSpan))
			{
				if (value == null)
					return TimeSpan.Zero;

                if (value is long)
                    return new TimeSpan((long)value);
                
                TimeSpan ts;
                if (TimeSpan.TryParse(value.ToString(), out ts))
                    return ts;

                return TimeSpan.Zero;
			}

#if NETFX_CORE
            TypeCode typeCode = conversionType.GetTypeCode();
#else
			TypeCode typeCode = Type.GetTypeCode(conversionType);
#endif
			switch(typeCode)
			{
				case TypeCode.Boolean:
                    return ToBoolean(value, DefaultBooleanValue);

				case TypeCode.Byte:
					try
					{
                        if (value is sbyte)
                            return unchecked((byte)(sbyte)value);
                        
						return Convert.ToByte(value, provider);
					}
					catch
					{
						return DefaultByteValue;
					}

				case TypeCode.Char:
					try
					{
						return Convert.ToChar(value, provider);
					}
					catch
					{
						return DefaultCharValue;
					}

				case TypeCode.DateTime:
                    try
                    {
                        return Convert.ToDateTime(value, provider);
                    }
                    catch
                    {
                        return DefaultDateTimeValue;
                    }

				case TypeCode.Decimal:
                    if (value is string)
                    {
                        NumberFormatInfo nfi = provider as NumberFormatInfo;
                        if (nfi == null)
                        {
                            nfi = CultureInfo.CurrentCulture.NumberFormat;
                        }
                        value = ((string)value).Replace(nfi.CurrencySymbol, "");
                    }
					try
					{
						return Convert.ToDecimal(value, provider);
					}
					catch
					{
						return DefaultDecimalValue;
					}

				case TypeCode.Double:
					try
					{
						return Convert.ToDouble(value, provider);
					}
					catch
					{
						return DefaultDoubleValue;
					}

				case TypeCode.Int16:
					try
					{
                        if (value is ushort)
                            return unchecked((short)(ushort)value);
                        
                        return Convert.ToInt16(value, provider);
					}
					catch
					{
						return DefaultInt16Value;
					}

				case TypeCode.Int32:
					try
					{
                        if (value is uint)
                            return unchecked((int)(uint)value);
                        
                        return Convert.ToInt32(value, provider);
					}
					catch
					{
						return DefaultInt32Value;
					}

				case TypeCode.Int64:
					try
					{
                        if (value is ulong)
                            return unchecked((long)(ulong)value);

                        if (value is TimeSpan)
                            return ((TimeSpan)value).Ticks;

                        return Convert.ToInt64(value, provider);
					}
					catch
					{
						return DefaultInt64Value;
					}

				case TypeCode.SByte:
					try
					{
                        if (value is byte)
                            return unchecked((sbyte)(byte)value);
                        
                        return Convert.ToSByte(value, provider);
					}
					catch
					{
						return DefaultSByteValue;
					}

				case TypeCode.Single:
					try
					{
						return Convert.ToSingle(value, provider);
					}
					catch
					{
						return DefaultSingleValue;
					}

				case TypeCode.String:
					if (value is byte[])
						return ToHexa((byte[])value);
					
					try
					{
                        if (value == null)
                            return null; // stupid ToString(object) returns string.Empty...

						return Convert.ToString(value, provider);
					}
					catch
					{
						return null;
					}

				case TypeCode.UInt16:
					try
					{
                        if (value is short)
                            return unchecked((ushort)(short)value);
                        
                        return Convert.ToUInt16(value, provider);
					}
					catch
					{
						return DefaultUInt16Value;
					}

				case TypeCode.UInt32:
					try
					{
                        if (value is int)
                            return unchecked((uint)(int)value);
                        
                        return Convert.ToUInt32(value, provider);
					}
					catch
					{
						return DefaultUInt32Value;
					}

				case TypeCode.UInt64:
					try
					{
                        if (value is long)
                            return unchecked((ulong)(long)value);
                        
                        return Convert.ToUInt64(value, provider);
					}
					catch
					{
						return DefaultUInt64Value;
					}


				case TypeCode.Object:
                    if (value == null)
                    {
#if NETFX_CORE
                        if (conversionType.IsValueType())
                            return Activator.CreateInstance(conversionType);
#else
                        if (conversionType.IsValueType)
                            return conversionType.Assembly.CreateInstance(conversionType.FullName);
#endif

                        return null;
                    }

#if NETFX_CORE
                    if ((conversionType.IsGenericType()) && (!conversionType.IsGenericTypeDefinition()))
#else
                    if ((conversionType.IsGenericType) && (!conversionType.IsGenericTypeDefinition))
#endif
                    {
                        if (conversionType.Name == typeof(Nullable<>).Name)
                        {
                            Type nt = typeof(Nullable<>).MakeGenericType(conversionType.GetGenericArguments()[0]);
                            if (String.Empty.Equals(value))
                                return Activator.CreateInstance(nt);

                            return Activator.CreateInstance(nt, ChangeType(value, conversionType.GetGenericArguments()[0], provider));
                        }
                    }
#if !NETFX_CORE
					IConvertible convertible = value as IConvertible;
                    if (CanChangeType(conversionType) && (convertible != null))
                    {
                        if ((!String.Empty.Equals(value)) || (!conversionType.IsValueType))
                        {
                            try
                            {
                                return Convert.ChangeType(value, conversionType, provider);
                            }
                            catch
                            {
                                // go on
                            }
                        }
					}
#endif

#if !SILVERLIGHT && !NETFX_CORE
                    try
                    {
                        TypeConverter cvt = TypeDescriptor.GetConverter(value);
                        if ((cvt != null) && (!IsChangeTypeConverter(cvt)) && (cvt.CanConvertTo(conversionType)))
                            return cvt.ConvertTo(null, provider as CultureInfo, value, conversionType);
                        

                        cvt = TypeDescriptor.GetConverter(conversionType);
                        if ((cvt != null) && (cvt.CanConvertFrom(value.GetType())))
                            return cvt.ConvertFrom(null, provider as CultureInfo, value); 
                        
                    }
                    catch
                    {
                        if (conversionType.IsValueType)
                            return conversionType.Assembly.CreateInstance(conversionType.FullName);

                        return null;
                    }
#endif

                    object outValue;
                    // don't use provider here because TryParse does not support it
                    if (TryParse(String.Format("{0}", value), conversionType, provider, out outValue))
                        return outValue;

#if NETFX_CORE
                    if (conversionType.IsValueType())
                        return Activator.CreateInstance(conversionType);
#else
                    if (conversionType.IsValueType)
                        return conversionType.Assembly.CreateInstance(conversionType.FullName);
#endif                    
                    return null;

				default:
					if (value == null)
						throw new ArgumentException(SR.GetString("unhandledConversionTypeNull", conversionType.FullName));
						
					throw new ArgumentException(SR.GetString("unhandledConversionType", conversionType.FullName, value.ToString(), value.GetType().FullName));

			}
		}

#if !SILVERLIGHT && !NETFX_CORE

        /// <summary>
        /// Converts the specified object of to its typed equivalent.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="dbType">The target type.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <param name="value">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType(object obj, DbType dbType, IFormatProvider provider, out object value)
        {
            return TryChangeType(obj, ToType(dbType), provider, out value);
        }
#endif

        /// <summary>
        /// Converts the specified object of to its typed equivalent.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <param name="value">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType<T>(object obj, out T value)
        {
            return TryChangeType(obj, null, out value);
        }

        /// <summary>
        /// Converts the specified object of to its typed equivalent.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <param name="value">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType<T>(object obj, IFormatProvider provider, out T value)
        {
            object v;
            bool b = TryChangeType(obj, typeof(T), provider, out v);
            if (b)
            {
                value = (T)v;
            }
            else
            {
                value = default(T);
            }
            return b;
        }

        /// <summary>
        /// Converts the specified object of to its typed equivalent.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="type">The target type. May not be null.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <param name="value">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType(object obj, Type type, IFormatProvider provider, out object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type == typeof(object))
            {
                value = obj;
                return true;
            }

            if (obj == null)
                return TryParse(null, type, provider, out value);

            if (obj.GetType().IsAssignableFrom(type))
            {
                value = obj;
                return true;
            }

            if (IsNullable(type))
            {
                if ((obj == null) || (String.Empty.Equals(obj)))
                {
                    value = null;
                    return true;
                }

                object vtValue;
                Type vtType = type.GetGenericArguments()[0];
                if (TryChangeType(obj, vtType, provider, out vtValue))
                {
                    Type nt = typeof(Nullable<>).MakeGenericType(vtType);
                    value = Activator.CreateInstance(nt, vtValue);
                    return true;
                }
                value = null;
                return false;
            }

            string s = obj as string;
            if (s != null)
                return TryParse(s, type, provider, out value);

            // not string based conversion start here
            byte[] bytes = obj as byte[];
            if (bytes != null)
            {
                if ((bytes.Length == 4) && (type == typeof(int)))
                {
                    value = BitConverter.ToInt32(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 1) && (type == typeof(bool)))
                {
                    value = BitConverter.ToBoolean(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 8) && (type == typeof(double)))
                {
                    value = BitConverter.ToDouble(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 4) && (type == typeof(float)))
                {
                    value = BitConverter.ToSingle(bytes, 0);
                    return true;
                }

                if ((bytes.Length > 0) && (type == typeof(string)))
                {
                    value = BitConverter.ToString(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 4) && (type == typeof(uint)))
                {
                    value = BitConverter.ToUInt32(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 2) && (type == typeof(short)))
                {
                    value = BitConverter.ToInt16(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 2) && (type == typeof(ushort)))
                {
                    value = BitConverter.ToUInt16(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 8) && (type == typeof(long)))
                {
                    value = BitConverter.ToInt64(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 8) && (type == typeof(ulong)))
                {
                    value = BitConverter.ToUInt64(bytes, 0);
                    return true;
                }

                if ((bytes.Length == 2) && (type == typeof(char)))
                {
                    value = BitConverter.ToChar(bytes, 0);
                    return true;
                }
            }

            if (type == typeof(Guid))
            {
                if ((bytes != null) && (bytes.Length == 16))
                {
                    value = new Guid(bytes);
                    return true;
                }
            }
#if !SILVERLIGHT
#if NETFX_CORE
            else if (type.IsEnum())
#else
            else if (type.IsEnum)
#endif
            {
                object e;
                bool ret = EnumTryParse(type, String.Format(provider, "{0}", obj), out e);
                if (ret)
                {
                    value = e;
                }
                else
                {
                    value = null;
                }
                return ret;
            }
#endif
            else if (type == typeof(byte[]))
            {
                if (obj is Guid)
                {
                    value = ((Guid)obj).ToByteArray();
                    return true;
                }

#if !SILVERLIGHT && !NETFX_CORE
                IBinarySerialize bs = obj as IBinarySerialize; // sql geometry, geography, hierarchyid
                if (bs != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            bs.Write(writer);
                            value = stream.ToArray();
                            return true;
                        }
                    }
                }
#endif
            }
            else if (type == typeof(IntPtr))
            {
                if (IntPtr.Size == 4)
                {
                    object typed;
                    bool ret = TryChangeType(obj, typeof(int), provider, out typed);
                    if (ret)
                    {
                        value = new IntPtr((int)typed);
                    }
                    else
                    {
                        value = null;
                    }
                    //value = ret ? new IntPtr((int)value) : IntPtr.Zero;
                    return ret;
                }
                else //if (IntPtr.Size == 8)
                {
                    object typed;
                    bool ret = TryChangeType(obj, typeof(long), provider, out typed);
                    if (ret)
                    {
                        value = new IntPtr((long)typed);
                    }
                    else
                    {
                        value = null;
                    }
                    //value = ret ? new IntPtr((long)value) : IntPtr.Zero;
                    return ret;
                }
            }
#if !SILVERLIGHT && !NETFX_CORE
            else if (type == typeof(XmlDocument))
            {
                XmlDocument typed;
                bool ret = TryChangeType(obj, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }
#endif
            else if (type == typeof(CultureInfo))
            {
                CultureInfo typed;
                bool ret = TryChangeType(obj, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }
            else if (type == typeof(Encoding))
            {
                Encoding typed;
                bool ret = TryChangeType(obj, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }
            else if (type == typeof(Version))
            {
                Version typed;
                bool ret = TryChangeType(obj, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }
            else if ((type == typeof(long)) && (obj.GetType() == typeof(TimeSpan)))
            {
                value = ((TimeSpan)obj).Ticks;
                return true;
            }
            else if ((type == typeof(TimeSpan)) && (obj is long))
            {
                value = new TimeSpan((long)obj);
                return true;
            }

            if (obj is IntPtr)
            {
                obj = IntPtr.Size == 4 ? ((IntPtr)obj).ToInt32() : ((IntPtr)obj).ToInt64();
                return TryChangeType(obj, type, provider, out value);
            }

            if (TryParse(String.Format(provider, "{0}", obj), type, provider, out value))
                return true;

#if !NETFX_CORE
			IConvertible convertible = obj as IConvertible;
            if ((convertible != null) && CanChangeType(type))
            {
                if ((!String.Empty.Equals(obj)) || (!type.IsValueType))
                {
                    try
                    {
                        value = Convert.ChangeType(obj, type, provider);
                        return true;
                    }
                    catch
                    {
                        // do nothing
                    }
                }
            }
#endif

#if !SILVERLIGHT && !NETFX_CORE
            try
            {
                TypeConverter cvt = TypeDescriptor.GetConverter(obj);
                if ((cvt != null) && (!IsChangeTypeConverter(cvt)) && (cvt.CanConvertTo(type)))
                {
                    value = cvt.ConvertTo(null, provider as CultureInfo, obj, type);
                    return true;
                }

                cvt = TypeDescriptor.GetConverter(type);
                if ((cvt != null) && (cvt.CanConvertFrom(obj.GetType())))
                {
                    value = cvt.ConvertFrom(null, provider as CultureInfo, obj);
                    return true;
                }
            }
            catch
            {
                // do nothing
            }
#endif

#if NETFX_CORE
            value = type.IsValueType() ? Activator.CreateInstance(type) : null;
#else
            value = type.IsValueType ? type.Assembly.CreateInstance(type.FullName) : null;
#endif
            return false;
        }
        
        /// <summary>
        /// Converts the specified string representation of an object of the specified type to its typed equivalent using the TryParse method if it exists on the type.
        /// </summary>
        /// <param name="s">A string to convert. May be null.</param>
        /// <param name="type">The target type. May not be null.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <param name="value">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the string was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string s, Type type, IFormatProvider provider, out object value)
        {
            return TryParse(s, type, provider, null, out value);
        }

        private static bool NormalizeHexString(ref string s)
        {
            if (s == null)
                return false;

            if (s.Length > 0)
            {
                if ((s[0] == 'x') || (s[0] == 'X'))
                {
                    s = s.Substring(1);
                    return true;
                }

                if (s.Length > 1)
                {
                    if ((s[0] == '0') && ((s[1] == 'x') || (s[1] == 'X')))
                    {
                        s = s.Substring(2);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Converts the specified string representation of an object of the specified type to its typed equivalent using the TryParse method if it exists on the type.
        /// </summary>
        /// <param name="s">A string to convert. May be null.</param>
        /// <param name="type">The target type. May not be null.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <param name="styles">The styles to use. Depends on the target type.</param>
        /// <param name="value">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the string was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string s, Type type, IFormatProvider provider, object styles, out object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type == typeof(string))
            {
                value = s;
                return true;
            }

#if NETFX_CORE
            if (!type.IsValueType() && string.IsNullOrEmpty(s))
#else
            if (!type.IsValueType && String.IsNullOrEmpty(s))
#endif
            {
                value = null;
                return true;
            }

            if (IsNullable(type))
            {
                if (String.IsNullOrEmpty(s))
                {
                    value = null;
                    return true;
                }

                object vtValue;
                Type vtType = type.GetGenericArguments()[0];
                if (TryParse(s, vtType, provider, out vtValue))
                {
                    Type nt = typeof(Nullable<>).MakeGenericType(vtType);
                    value = Activator.CreateInstance(nt, vtValue);
                    return true;
                }
                value = null;
                return false;
            }

            bool ret;
            if (type == typeof(bool))
            {
                bool typed;
                ret = TryParse(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

#if NETFX_CORE
            if (type.IsEnum())
#else
            if (type.IsEnum)
#endif
            {
#if !SILVERLIGHT
                object typed;
                ret = EnumTryParse(type, s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
#else
                value = ToEnum(s, type);
                return true;
#endif
            }

            NumberStyles defaultIntegerStyles = styles == null ? NumberStyles.Integer : (NumberStyles)styles;
            if (type == typeof(int))
            {
                int typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = Int32.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(DateTime))
            {
                DateTime typed;
                ret = DateTime.TryParse(s, provider, styles == null ? DateTimeStyles.None : (DateTimeStyles)styles, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(decimal))
            {
                decimal typed;
                defaultIntegerStyles = styles == null ? NumberStyles.Number : (NumberStyles)styles;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = Decimal.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(Guid))
            {
                Guid typed;
                ret = TryParse(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(double))
            {
                double typed;
                ret = Double.TryParse(s, styles == null ? NumberStyles.Float | NumberStyles.AllowThousands : (NumberStyles)styles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(float))
            {
                float typed;
                ret = Single.TryParse(s, styles == null ? NumberStyles.Float | NumberStyles.AllowThousands : (NumberStyles)styles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(uint))
            {
                uint typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = UInt32.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(short))
            {
                short typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = Int16.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(ushort))
            {
                ushort typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = UInt16.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(long))
            {
                long typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = Int64.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(ulong))
            {
                ulong typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = UInt64.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(byte))
            {
                byte typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = Byte.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(sbyte))
            {
                sbyte typed;
                if (NormalizeHexString(ref s))
                {
                    defaultIntegerStyles = NumberStyles.AllowHexSpecifier;
                }
                ret = SByte.TryParse(s, defaultIntegerStyles, provider, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(DateTimeOffset))
            {
                DateTimeOffset typed;
                ret = DateTimeOffset.TryParse(s, provider, styles == null ? DateTimeStyles.None : (DateTimeStyles)styles, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(TimeSpan))
            {
                TimeSpan typed;
                ret = TimeSpan.TryParse(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(char))
            {
                char typed;
                ret = Char.TryParse(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(byte[]))
            {
                if ((s != null) && (s.IndexOf(',') > 0))
                {
                    value = ToBytesFromArray(s, ',');
                }
                else
                {
                    value = ToBytesFromHexa(s);
                }
                return true;
            }

#if !NETFX_CORE
            if (type == typeof(IPAddress))
            {
                IPAddress typed;
                ret = IPAddress.TryParse(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }
#endif

#if !SILVERLIGHT && !NETFX_CORE
            if (type == typeof(XmlDocument))
            {
                XmlDocument typed;
                ret = TryChangeType(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }
#endif
            if (type == typeof(CultureInfo))
            {
                CultureInfo typed;
                ret = TryChangeType(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(Encoding))
            {
                Encoding typed;
                ret = TryChangeType(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

            if (type == typeof(Version))
            {
                Version typed;
                ret = TryChangeType(s, out typed);
                if (ret)
                {
                    value = typed;
                }
                else
                {
                    value = null;
                }
                return ret;
            }

#if NETFX_CORE
            object defaultValue = type.IsValueType() ? Activator.CreateInstance(type) : null;
            MethodInfo mi = type.GetRuntimeMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
#else
            object defaultValue = type.IsValueType ? type.Assembly.CreateInstance(type.FullName) : null;
            MethodInfo mi = type.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), type.MakeByRefType() }, null);
#endif
            if ((mi == null) || (mi.ReturnType != typeof(bool)))
            {
                value = defaultValue;
                return false;
            }

            object refValue = defaultValue;
            object[] parameters = new object[] { s, refValue };
            ret = (bool)mi.Invoke(null, parameters);
            value = parameters[1];
            return ret;
		}

        /// <summary>
        /// Nullifies the specified string.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <param name="trim">if set to <c>true</c> the string will be trimmed before being checked for emptyness.</param>
        /// <returns>Null if the string is null or empty; otherwise the original string.</returns>
		public static string Nullify(string text, bool trim)
		{
			if (text == null)
				return text;
			
			string strim = text.Trim();
			if (strim.Length == 0)
				return null;
			
			if (trim)
				return strim;
			
			return text;
		}

#if !SILVERLIGHT
        private static object EnumToObject(Type underlyingType, string input)
        {
            if (underlyingType == typeof(int))
            {
                int s;
                if (Int32.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(uint))
            {
                uint s;
                if (UInt32.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(ulong))
            {
                ulong s;
                if (UInt64.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(long))
            {
                long s;
                if (Int64.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(short))
            {
                short s;
                if (Int16.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(ushort))
            {
                ushort s;
                if (UInt16.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(byte))
            {
                byte s;
                if (Byte.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(sbyte))
            {
                sbyte s;
                if (SByte.TryParse(input, out s))
                    return s;
            }

            return null;
        }

        private static bool StringToEnum(Type type, Type underlyingType, string[] names, Array values, string input, out object value)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (String.Compare(names[i], input, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = values.GetValue(i);
                    return true;
                }
            }

            for (int i = 0; i < values.GetLength(0); i++)
            {
                object valuei = values.GetValue(i);
                ulong ul = EnumToUInt64(valuei);
                if (String.Compare(ul.ToString(), input, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = valuei;
                    return true;
                }
            }

            if (Char.IsDigit(input[0]) || input[0] == '-' || input[0] == '+')
            {
                object obj = EnumToObject(underlyingType, input);
                if (obj == null)
                {
                    value = Activator.CreateInstance(type);
                    return false;
                }
                value = obj;
                return true;
            }

            value = Activator.CreateInstance(type);
            return false;
        }
        
        /// <summary>
        /// Converts the string representation of an enum to its Enum equivalent value. A return value indicates whether the operation succeeded.
        /// This method does not rely on Enum.Parse and therefore will never raise any first or second chance exception.
        /// </summary>
        /// <param name="type">The enum target type. May not be null.</param>
        /// <param name="input">The input text. May be null.</param>
        /// <param name="value">When this method returns, contains Enum equivalent value to the enum contained in input, if the conversion succeeded.</param>
        /// <returns>
        /// true if s was converted successfully; otherwise, false.
        /// </returns>
        public static bool EnumTryParse(Type type, string input, out object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");

#if NETFX_CORE
            if (!type.IsEnum())
                throw new ArgumentException(null, "type");
#else
            if (!type.IsEnum)
                throw new ArgumentException(null, "type");
#endif

            if (input == null)
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            input = input.Trim();
            if (input.Length == 0)
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                ulong ulx;
                if (UInt64.TryParse(input.Substring(2), NumberStyles.HexNumber, null, out ulx))
                {
                    value = ToEnum(ulx.ToString(CultureInfo.InvariantCulture), type);
                    return true;
                }
            }

            string[] names = Enum.GetNames(type);
            if (names.Length == 0)
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            Type underlyingType = Enum.GetUnderlyingType(type);
            Array values = Enum.GetValues(type);
            // some enums like System.CodeDom.MemberAttributes *are* flags but are not declared with Flags...
            if ((!type.IsDefined(typeof(FlagsAttribute), true)) && (input.IndexOfAny(_enumSeperators) < 0))
                return StringToEnum(type, underlyingType, names, values, input, out value);

            // multi value enum
            string[] tokens = input.Split(_enumSeperators, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            ulong ul = 0;
            foreach (string tok in tokens)
            {
                string token = tok.Trim(); // NOTE: we don't consider empty tokens as errors
                if (token.Length == 0)
                    continue;

                object tokenValue;
                if (!StringToEnum(type, underlyingType, names, values, token, out tokenValue))
                {
                    value = Activator.CreateInstance(type);
                    return false;
                }

                ulong tokenUl;
#if NETFX_CORE
                switch (GetObjectTypeCode(tokenValue))
#else
                switch (Convert.GetTypeCode(tokenValue))
#endif
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                        tokenUl = (ulong)Convert.ToInt64(tokenValue, CultureInfo.InvariantCulture);
                        break;

                    //case TypeCode.Byte:
                    //case TypeCode.UInt16:
                    //case TypeCode.UInt32:
                    //case TypeCode.UInt64:
                    default:
                        tokenUl = Convert.ToUInt64(tokenValue, CultureInfo.InvariantCulture);
                        break;
                }

                ul |= tokenUl;
            }
            value = Enum.ToObject(type, ul);
            return true;
        }
#endif

        /// <summary>
        /// Converts the input value to an equivalent enum value.
        /// If an error occurs, a computed default value of the target enum type will be returned.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <param name="enumType">The target enum type. May not be null.</param>
        /// <returns>An enum value of the target enum type.</returns>
        public static object ToEnum(string text, Type enumType)
		{
			if (enumType == null)
				throw new ArgumentNullException("enumType");

#if !SILVERLIGHT
            object value;
            EnumTryParse(enumType, text, out value);
            return value;
#else
            text = Nullify(text, true);
			if (text == null)
				return Activator.CreateInstance(enumType);

			try
			{
				return Enum.Parse(enumType, text, true);
			}
			catch
			{
                return Activator.CreateInstance(enumType);
            }
#endif
        }

        /// <summary>
        /// Converts the input value to an equivalent enum value.
        /// If an error occurs, the value parameter will be returned.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <param name="defaultValue">The value to use if an error occurs.</param>
        /// <returns>An enum value of the target enum type.</returns>
        public static Enum ToEnum(string text, Enum defaultValue)
		{
			if (defaultValue == null)
				throw new ArgumentNullException("defaultValue");

#if !SILVERLIGHT
            object value;
            if (EnumTryParse(defaultValue.GetType(), text, out value))
                return (Enum)value;

            return defaultValue;
#else
            text = Nullify(text, true);
			if (text == null)
				return defaultValue;

			try
			{
				return (Enum)Enum.Parse(defaultValue.GetType(), text, true);
			}
			catch
			{
				return defaultValue;
			}
#endif
		}
        
#if !SILVERLIGHT && !NETFX_CORE
        /// <summary>
        /// Converts a specified value to an XmlDocument.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="document">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType(object value, out XmlDocument document)
        {
            if (value == null)
            {
                document = null;
                return true;
            }

            if (value.GetType().IsAssignableFrom(typeof(XmlDocument)))
            {
                document = (XmlDocument)value;
                return true;
            }

            XmlReader xr = value as XmlReader;
            if (xr != null)
            {
                try
                {
                    document = new XmlDocument();
                    document.Load(xr);
                    return true;
                }
                catch
                {
                    document = null;
                    return false;
                }
            }

            TextReader tr = value as TextReader;
            if (tr != null)
            {
                try
                {
                    document = new XmlDocument();
                    document.Load(tr);
                    return true;
                }
                catch
                {
                    document = null;
                    return false;
                }
            }

            Stream stream = value as Stream;
            if (stream != null)
            {
                try
                {
                    document = new XmlDocument();
                    document.Load(stream);
                    return true;
                }
                catch
                {
                    document = null;
                    return false;
                }
            }

            string xml = value as string;
            if (xml != null)
            {
                if (xml.TrimStart().StartsWith("&lt;"))
                {
                    xml = UnescapeXml(xml);
                }
                try
                {
                    document = new XmlDocument();
                    document.LoadXml(xml);
                    return true;
                }
                catch
                {
                }
            }
            document = null;
            return false;
        }

        /// <summary>
        /// Converts a specified value to an XmlDocument.
        /// If an error occurs, the value parameter will be returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The XmlDocument equivalent to the input value.
        /// </returns>
        public static XmlDocument ToXmlDocument(object value, XmlDocument defaultValue)
        {
            XmlDocument doc = new XmlDocument();
            if (value == null)
                return doc;

            try
            {
                if (value is XmlReader)
                {
                    doc.Load((XmlReader)value);
                    return doc;
                }

                if (value is TextReader)
                {
                    doc.Load((TextReader)value);
                    return doc;
                }

                if (value is Stream)
                {
                    doc.Load((Stream)value);
                    return doc;
                }

                string xml = ChangeType<string>(value);
                // try to sniff if it's escaped or not
                if (xml.TrimStart().StartsWith("&lt;"))
                {
                    xml = UnescapeXml(xml);
                }
                doc.LoadXml(xml);
                return doc;
            }
            catch
            {
                return defaultValue;
            }
        }
#endif
        
		/// <summary>
        /// Converts the specified value to an equivalent Boolean value.
        /// If an error occurs, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="obj">The input value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The Boolean equivalent of the input value.
        /// </returns>
        public static bool ToBoolean(object obj, bool defaultValue)
        {
            if (obj == null)
                return defaultValue;

            if (obj is bool)
                return ((bool)obj);

#if !NETFX_CORE
            if (obj is BooleanEnum)
                return (((BooleanEnum)obj) == BooleanEnum.True);
#endif

            if (obj is bool?)
            {
                bool? b = (bool?)obj;
                return b.GetValueOrDefault(defaultValue);
            }

            return ToBoolean(obj.ToString(), defaultValue);
        }

        /// <summary>
        /// Converts the specified string representation of a logical value to its System.Boolean
        /// equivalent. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="text">The input value.</param>
        /// <param name="value">When this method returns, if the conversion succeeded, contains the converted value.</param>
        /// <returns>
        /// true if value was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string text, out bool value)
        {
            text = Nullify(text, true);
            value = false;
            if (String.IsNullOrEmpty(text))
                return false;

            if (text == "0")
            {
                value = false;
                return true;
            }

            if (text == "1")
            {
                value = true;
                return true;
            }

            if (text == "-1") // excel woes
            {
                value = true;
                return true;
            }

#if NETFX_CORE
            string lower = text.ToLower();
#else
            string lower = text.ToLower(CultureInfo.CurrentCulture);
#endif
            switch (lower)
            {
                case "true":
                case "t":
                case "yes":
                case "y": // oracle woes
                    value = true;
                    return true;

                case "false":
                case "f":
                case "no":
                case "n": // oracle woes
                    value = false;
                    return true;

                default:
                    if (text.StartsWith("true")) // MVC
                    {
                        value = true;
                        return true;
                    }

                    if (text.StartsWith("false"))
                    {
                        value = false;
                        return true;
                    }

                    if (Boolean.TryParse(text, out value))
                        return true;

                    int i;
                    if (Int32.TryParse(text, out i))
                    {
                        value = i != 0;
                        return true;
                    }
                    return false;
            }
        }

        /// <summary>
        /// Converts the specified value to an equivalent Boolean value.
        /// If an error occurs, the defaultValue parameter will be returned.
        /// </summary>
        /// <param name="text">The input value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The Boolean equivalent of the input value.
        /// </returns>
        public static bool ToBoolean(string text, bool defaultValue)
		{
            if (text == null)
                return defaultValue;

            text = text.Trim();

            if (text == "0")
                return false;

            if (text == "1")
                return true;

            if (text == "-1") // excel woes
                return true;

#if NETFX_CORE
            string l = text.Trim().ToLower();
#else
            string l = text.Trim().ToLower(CultureInfo.CurrentCulture);
#endif
            switch (l)
            {
                case "true":
                case "yes":
                case "y": // oracle woes
                    return true;

                case "false":
                case "no":
                case "n": // oracle woes
                    return false;

                default:
                    if (text.StartsWith("true")) // MVC
                        return true;

                    if (text.StartsWith("false"))
                        return false;

                    bool b;
                    if (Boolean.TryParse(text, out b))
                        return b;

                    int i;
                    if (Int32.TryParse(text, out i))
                        return i != 0;

                    return defaultValue;
            }
		}

        /// <summary>
        /// Converts the specified string representation of a logical value to its System.Guid
        /// equivalent. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="text">The input value.</param>
        /// <param name="value">When this method returns, if the conversion succeeded, contains the converted value.</param>
        /// <returns>
        /// true if value was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string text, out Guid value)
        {
            value = Guid.Empty;
            if (String.IsNullOrEmpty(text))
                return false;

            // we try to avoid simple exceptions
            if ((text.Length != 38) && (text.Length != 36) && (text.Length != 32))
                return false;

            if ((String.Compare(text, "new", StringComparison.CurrentCultureIgnoreCase) == 0) ||
                (String.Compare(text, "newid", StringComparison.CurrentCultureIgnoreCase) == 0) ||
                (String.Compare(text, "newguid", StringComparison.CurrentCultureIgnoreCase) == 0))
            {
                value = Guid.NewGuid();
                return true;
            }

            try
            {
                value = new Guid(text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Converts the specified value to an equivalent Guid value.
        /// If an error occurs, the value parameter will be returned.
        /// Special input values 'new', 'newid', 'newguid' will generate a new Guid.
        /// </summary>
        /// <param name="text">The input value.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Guid equivalent of the input value.</returns>
        public static Guid ToGuid(string text, Guid value)
		{
            if (String.IsNullOrEmpty(text))
				return value;

            // we try to avoid simple exceptions
            if ((text.Length != 38) && (text.Length != 36) && (text.Length != 32))
                return value;

			try
			{
                if ((String.Compare(text, "new", StringComparison.CurrentCultureIgnoreCase) == 0) ||
                    (String.Compare(text, "newid", StringComparison.CurrentCultureIgnoreCase) == 0) ||
                    (String.Compare(text, "newguid", StringComparison.CurrentCultureIgnoreCase) == 0))
					return Guid.NewGuid();
				
                return new Guid(text);
			}
			catch
			{
				return value;
			}
		}
        
        /// <summary>
        /// Converts the specified value to an equivalent Int32 value.
        /// If an error occurs, the value parameter will be returned.
        /// </summary>
        /// <param name="text">The input value.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Int32 equivalent of the input value.</returns>
        public static int ToInt32(string text, int value)
		{
			return ToInt32(text, value, null);
		}

        /// <summary>
        /// Converts the specified value to an equivalent Int32 value.
        /// If an error occurs, the value parameter will be returned.
        /// </summary>
        /// <param name="text">The input value.</param>
        /// <param name="value">The value.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>The Int32 equivalent of the input value.</returns>
        public static int ToInt32(string text, int value, IFormatProvider provider)
		{
			if (text == null)
				return value;

            int i;
			if (text.Length > 1)
			{
				if ((text[0] == 'x') || (text[0] == 'X'))
				{
                    if (Int32.TryParse(text.Substring(1), NumberStyles.AllowHexSpecifier, provider, out i))
                        return i;

                    return value;
				}
				if ((text[0] == '0') &&
					((text[1] == 'x') || (text[1] == 'X')))
				{
                    if (Int32.TryParse(text.Substring(2), NumberStyles.AllowHexSpecifier, provider, out i))
                        return i;

                    return value;
                }
			}
            if (Int32.TryParse(text, NumberStyles.Any, provider, out i))
                return i;

			return value;
		}
        
#if !SILVERLIGHT && !NETFX_CORE

        /// <summary>
        /// Unescapes an Xml text.
        /// </summary>
        /// <param name="text">The input text. May be null.</param>
        /// <returns>An unescaped Xml text.</returns>
        public static string UnescapeXml(string text)
        {
            if (text == null)
                return null;

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml("<e a=\"" + text.Replace("\"", "&quot;") + "\"/>");
                return doc.DocumentElement.Attributes[0].Value;
            }
            catch
            {
                throw new ArgumentException(null, "text");
            }
        }
#endif

        /// <summary>
        /// Defines the options for decamelization.
        /// </summary>
        [Flags]
        public enum DecamelizeOptions
        {
            /// <summary>
            /// No option is defined.
            /// </summary>
            None = 0,
            
            /// <summary>
            /// First character will be upper case.
            /// </summary>
            ForceFirstUpper = 1,
            
            /// <summary>
            /// Characters beyond the first will be lower case.
            /// </summary>
            ForceRestLower = 2,
            
            /// <summary>
            /// Unescape unicode encoding (format is \u0000).
            /// </summary>
            UnescapeUnicode = 4,
            
            /// <summary>
            /// Unescape hexadecimal encoding (format is \x0000).
            /// </summary>
            UnescapeHexadecimal = 8,

            /// <summary>
            /// Replaces spaces by underscore.
            /// </summary>
            ReplaceSpacesByUnderscore = 0x10,

            /// <summary>
            /// Replaces spaces by minus.
            /// </summary>
            ReplaceSpacesByMinus = 0x20,

            /// <summary>
            /// Replaces spaces by dot.
            /// </summary>
            ReplaceSpacesByDot = 0x40,
            
            /// <summary>
            /// Keep first underscores sticked as is.
            /// </summary>
            KeepFirstUnderscores = 0x80,

            /// <summary>
            /// Numbers are not considered as separators.
            /// </summary>
            DontDecamelizeNumbers = 0x100,

            /// <summary>
            /// Keep indices used by the String.Format method.
            /// </summary>
            KeepFormattingIndices = 0x200,

            /// <summary>
            /// Defines the default options.
            /// </summary>
            Default = ForceFirstUpper | UnescapeUnicode | UnescapeHexadecimal | KeepFirstUnderscores,
        }
        
        /// <summary>
        /// Decamelizes the specified text using default decamelization options.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>A decamelized text.</returns>
        public static string Decamelize(string text)
        {
            return Decamelize(text, DecamelizeOptions.Default);
        }

        private static bool IsPureNumber(char c)
        {
            // note: we don't want to use Char.IsDigit nor Char.IsNumber
            return ((c >= '0') && (c <= '9'));
        }

        private static bool IsHexNumber(char c)
        {
            // note: we don't want to use Char.IsDigit nor Char.IsNumber
            return ((c >= '0') && (c <= '9')) || ((c >= 'a') && (c <= 'f')) || ((c >= 'A') && (c <= 'F'));
        }

        // format is _xXXXX_
        private static bool CanHexadecimalEscape(string text, int i)
        {
            return (((i + 6) < text.Length) && (text[i] == '_') && (text[i + 1] == 'x') && (text[i + 6] == '_') &&
                (IsHexNumber(text[i + 2])) &&
                (IsHexNumber(text[i + 3])) &&
                (IsHexNumber(text[i + 4])) &&
                (IsHexNumber(text[i + 5])));
        }

        private static char GetHexadecimalEscape(string text, ref int i)
        {
            string s = text[i + 2].ToString();
            s += text[i + 3].ToString();
            s += text[i + 4].ToString();
            s += text[i + 5].ToString();
            i += 6;
            return (char)Int32.Parse(s, NumberStyles.HexNumber);
        }

        // format is \uXXXX
        private static bool CanUnicodeEscape(string text, int i)
        {
            return (((i + 5) < text.Length) && (text[i] == '\\') && (text[i + 1] == 'u') &&
                (IsPureNumber(text[i + 2])) &&
                (IsPureNumber(text[i + 3])) &&
                (IsPureNumber(text[i + 4])) &&
                (IsPureNumber(text[i + 5])));
        }

        private static char GetUnicodeEscape(string text, ref int i)
        {
            string s = text[i + 2].ToString();
            s += text[i + 3].ToString();
            s += text[i + 4].ToString();
            s += text[i + 5].ToString();
            i += 5;
            return (char)Int32.Parse(s);
        }

        /// <summary>
        /// Decamelizes the specified text.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="options">The decamelization options.</param>
        /// <returns>A decamelized text.</returns>
        public static string Decamelize(string text, DecamelizeOptions options)
		{
			// input: a string like loadByWhateverStuff
			// output: a string like Load By Whatever Stuff
			// BBKing -> BBKing
			// BBOKing -> BboKing
			// LoadBy25Years -> Load By 25 Years
			// SoftFluent.PetShop -> Soft Fluent. Pet Shop
			// Data2_FileName -> Data 2 File Name
            // _WhatIs -> _What is
            // __WhatIs -> __What is
            // __What__Is -> __What is
            // MyParam1 -> My Param 1
            // MyParam1Baby -> My Param1 Baby (if DontDecamelizeNumbers)

			if (text == null)
				return null;

			if (text.Length == 0)
				return text;

			StringBuilder sb = new StringBuilder(text.Length);

			UnicodeCategory prevCategory;

			// 0=lower, 1=upper, 2=special char
            UnicodeCategory lastCategory = CharUnicodeInfo.GetUnicodeCategory(text[0]);
			prevCategory = lastCategory;
			if (lastCategory == UnicodeCategory.UppercaseLetter)
			{
				lastCategory = UnicodeCategory.LowercaseLetter;
			}

            int i = 0;
            bool firstIsStillUnderscore = (text[0] == '_');
            if (((options & DecamelizeOptions.UnescapeUnicode) == DecamelizeOptions.UnescapeUnicode) && (CanUnicodeEscape(text, 0)))
            {
                sb.Append(GetUnicodeEscape(text, ref i));
            }
            else if (((options & DecamelizeOptions.UnescapeHexadecimal) == DecamelizeOptions.UnescapeHexadecimal) && (CanHexadecimalEscape(text, 0)))
            {
                sb.Append(GetHexadecimalEscape(text, ref i));
            }
            else
            {
                if ((options & DecamelizeOptions.ForceFirstUpper) == DecamelizeOptions.ForceFirstUpper)
                {
                    sb.Append(Char.ToUpper(text[0]));
                }
                else
                {
                    sb.Append(text[0]);
                }
            }
			bool separated = false;
            bool keepFormat = (options & DecamelizeOptions.KeepFormattingIndices) == DecamelizeOptions.KeepFormattingIndices;

			for(i++; i < text.Length; i++)
			{
				char c = text[i];
                if (((options & DecamelizeOptions.UnescapeUnicode) == DecamelizeOptions.UnescapeUnicode) && (CanUnicodeEscape(text, i)))
                {
                    sb.Append(GetUnicodeEscape(text, ref i));
                    separated = true;
                }
                else if (((options & DecamelizeOptions.UnescapeHexadecimal) == DecamelizeOptions.UnescapeHexadecimal) && (CanHexadecimalEscape(text, i)))
                {
                    sb.Append(GetHexadecimalEscape(text, ref i));
                    separated = true;
                }
                else if (c == '_')
                {
                    if ((!firstIsStillUnderscore) || ((options & DecamelizeOptions.KeepFirstUnderscores) != DecamelizeOptions.KeepFirstUnderscores))
                    {
                        sb.Append(' ');
                        separated = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                    switch (category)
                    {
                        case UnicodeCategory.ClosePunctuation:
                        case UnicodeCategory.ConnectorPunctuation:
                        case UnicodeCategory.DashPunctuation:
                        case UnicodeCategory.EnclosingMark:
                        case UnicodeCategory.FinalQuotePunctuation:
                        case UnicodeCategory.Format:
                        case UnicodeCategory.InitialQuotePunctuation:
                        case UnicodeCategory.LineSeparator:
                        case UnicodeCategory.OpenPunctuation:
                        case UnicodeCategory.OtherPunctuation:
                        case UnicodeCategory.ParagraphSeparator:
                        case UnicodeCategory.SpaceSeparator:
                        case UnicodeCategory.SpacingCombiningMark:
                            if ((keepFormat) && (c == '{'))
                            {
                                while (c != '}')
                                {
                                    c = text[i++];
                                    sb.Append(c);
                                }
                                i--;
                                separated = true;
                                break;
                            }

                            if ((options & DecamelizeOptions.ForceRestLower) == DecamelizeOptions.ForceRestLower)
                            {
                                sb.Append(Char.ToLower(c));
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            sb.Append(' ');
                            separated = true;
                            break;

                        case UnicodeCategory.LetterNumber:
                        case UnicodeCategory.DecimalDigitNumber:
                        case UnicodeCategory.OtherNumber:

                        case UnicodeCategory.CurrencySymbol:
                        case UnicodeCategory.LowercaseLetter:
                        case UnicodeCategory.MathSymbol:
                        case UnicodeCategory.ModifierLetter:
                        case UnicodeCategory.ModifierSymbol:
                        case UnicodeCategory.NonSpacingMark:
                        case UnicodeCategory.OtherLetter:
                        case UnicodeCategory.OtherNotAssigned:
                        case UnicodeCategory.Control:
                        case UnicodeCategory.OtherSymbol:
                        case UnicodeCategory.Surrogate:
                        case UnicodeCategory.PrivateUse:
                        case UnicodeCategory.TitlecaseLetter:
                        case UnicodeCategory.UppercaseLetter:
                            if (((category != lastCategory) && (c != ' ')) && (IsNewCategory(category, options)))
                            {
                                if ((!separated) && (prevCategory != UnicodeCategory.UppercaseLetter) &&
                                    ((!firstIsStillUnderscore) || ((options & DecamelizeOptions.KeepFirstUnderscores) != DecamelizeOptions.KeepFirstUnderscores)))
                                {
                                    sb.Append(' ');
                                }

                                if ((options & DecamelizeOptions.ForceRestLower) != 0)
                                {
                                    sb.Append(Char.ToLower(c));
                                }
                                else
                                {
                                    sb.Append(Char.ToUpper(c));
                                }

                                char upper = Char.ToUpper(c);
                                category = CharUnicodeInfo.GetUnicodeCategory(upper);
                                if (category == UnicodeCategory.UppercaseLetter)
                                {
                                    lastCategory = UnicodeCategory.LowercaseLetter;
                                }
                                else
                                {
                                    lastCategory = category;
                                }
                            }
                            else
                            {
                                if ((options & DecamelizeOptions.ForceRestLower) != 0)
                                {
                                    sb.Append(Char.ToLower(c));
                                }
                                else
                                {
                                    sb.Append(c);
                                }
                            }
                            separated = false;
                            break;
                    }
                    firstIsStillUnderscore = firstIsStillUnderscore && (c == '_');
                    prevCategory = category;
                }
			}

            if ((options & DecamelizeOptions.ReplaceSpacesByUnderscore) == DecamelizeOptions.ReplaceSpacesByUnderscore)
                return sb.Replace(' ', '_').ToString();

            if ((options & DecamelizeOptions.ReplaceSpacesByMinus) == DecamelizeOptions.ReplaceSpacesByMinus)
                return sb.Replace(' ', '-').ToString();

            if ((options & DecamelizeOptions.ReplaceSpacesByDot) == DecamelizeOptions.ReplaceSpacesByDot)
                return sb.Replace(' ', '.').ToString();

			return sb.ToString();
		}

        private static bool IsNewCategory(UnicodeCategory category, DecamelizeOptions options)
        {
            if ((options & DecamelizeOptions.DontDecamelizeNumbers) == DecamelizeOptions.DontDecamelizeNumbers)
            {
                if ((category == UnicodeCategory.LetterNumber) ||
                    (category == UnicodeCategory.DecimalDigitNumber) ||
                    (category == UnicodeCategory.OtherNumber))
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// Gets the maximum bitness for an enum type.
        /// </summary>
        /// <param name="enumType">The input enum type. May not be null.</param>
        /// <returns>8, 16, 32, or 64 depending on the enum underlying type.</returns>
		public static int GetEnumMaxPower(Type enumType)
		{
			if (enumType == null)
				throw new ArgumentNullException("enumType");

#if NETFX_CORE
            if (!enumType.IsEnum())
                throw new ArgumentException(null, "enumType");
#else
			if (!enumType.IsEnum)
				throw new ArgumentException(null, "enumType");
#endif

            Type utype = Enum.GetUnderlyingType(enumType);
            return GetEnumUnderlyingTypeMaxPower(utype);
		}

        /// <summary>
        /// Gets the maximum bitness for an enum underlying type.
        /// </summary>
        /// <param name="underlyingType">The input enum underlying type. May not be null.</param>
        /// <returns>8, 16, 32, or 64 depending on the input type.</returns>
        public static int GetEnumUnderlyingTypeMaxPower(Type underlyingType)
        {
            if (underlyingType == null)
                throw new ArgumentNullException("underlyingType");

            if ((underlyingType == typeof(long)) ||
                (underlyingType == typeof(ulong)))
                return 64;

            if ((underlyingType == typeof(int)) ||
                (underlyingType == typeof(uint)))
                return 32;

            if ((underlyingType == typeof(short)) ||
                (underlyingType == typeof(ushort)))
                return 16;

            if ((underlyingType == typeof(byte)) ||
                (underlyingType == typeof(sbyte)))
                return 8;

            throw new ArgumentException(null, "underlyingType");
        }

        /// <summary>
        /// Determines whether the specified enum type has the Flags attribute.
        /// </summary>
        /// <param name="enumType">The type to check. May not be null.</param>
        /// <returns>
        /// 	<c>true</c> if the specified enum type has the Flags attribute; otherwise, <c>false</c>.
        /// </returns>
		public static bool IsFlagsEnum(Type enumType)
		{
			if (enumType == null)
				throw new ArgumentNullException("enumType");

#if NETFX_CORE
            if (!enumType.IsEnum())
                throw new ArgumentException(null, "enumType");
#else
			if (!enumType.IsEnum)
				throw new ArgumentException(null, "enumType");
#endif

			return enumType.IsDefined(typeof(FlagsAttribute), true);
		}

        /// <summary>
        /// Converts an enum value to an UInt64 value.
        /// </summary>
        /// <param name="value">The enum value. May not be null.</param>
        /// <returns>The UInt64 value equivalent to the input value.</returns>
		[CLSCompliant(false)]
		public static ulong EnumToUInt64(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

#if NETFX_CORE
            TypeCode typeCode = GetObjectTypeCode(value);
#else
			TypeCode typeCode = Convert.GetTypeCode(value);
#endif
			switch(typeCode)
			{
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                
                case TypeCode.String:
                default:
                    return ChangeType<ulong>(value, CultureInfo.InvariantCulture);
			}
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The input type. May not be null</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

#if NETFX_CORE
            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>);
#else
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
#endif
        }
        
        /// <summary>
        /// Returns an instance of the specified enumeration type set to the underlying type.
        /// </summary>
        /// <param name="enumType">The Enum type. May not be null.</param>
        /// <param name="value">The enum value. May not be null.</param>
        /// <returns>
        /// An instance of the enumeration set to value.
        /// </returns>
        public static object EnumToObject(Type enumType, object value)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

#if NETFX_CORE
            if (!enumType.IsEnum())
                throw new ArgumentException(null, "enumType");
#else
            if (!enumType.IsEnum)
                throw new ArgumentException(null, "enumType");
#endif

            if (value == null)
                throw new ArgumentNullException("value");

            Type underlyingType = Enum.GetUnderlyingType(enumType);
            if (underlyingType == typeof(long))
                return Enum.ToObject(enumType, ChangeType<long>(value));

            if (underlyingType == typeof(ulong))
                return Enum.ToObject(enumType, ChangeType<ulong>(value));

            if (underlyingType == typeof(int))
                return Enum.ToObject(enumType, ChangeType<int>(value));
            
            if ((underlyingType == typeof(uint)))
                return Enum.ToObject(enumType, ChangeType<uint>(value));

            if (underlyingType == typeof(short))
                return Enum.ToObject(enumType, ChangeType<short>(value));
            
            if (underlyingType == typeof(ushort))
                return Enum.ToObject(enumType, ChangeType<ushort>(value));
            
            if (underlyingType == typeof(byte))
                return Enum.ToObject(enumType, ChangeType<byte>(value));
                
            if (underlyingType == typeof(sbyte))
                return Enum.ToObject(enumType, ChangeType<sbyte>(value));

            throw new ArgumentException(null, "enumType");
        }

        /// <summary>
        /// Camelizes the specified name.
        /// </summary>
        /// <param name="name">The input name. If null, null will be returned.</param>
        /// <returns>The camelized name.</returns>
        public static string Camel(string name)
        {
            if (name == null)
                return null;

            if (name.Length == 0)
                return null;

#if NETFX_CORE
            return Char.ToLower(name[0]) + name.Substring(1);
#else
            return Char.ToLower(name[0], CultureInfo.CurrentCulture) + name.Substring(1);
#endif
        }

        /// <summary>
        /// Converts the specified text to an equivalent CultureInfo instance.
        /// The text can be a culture name of the string representation of an lcid.
        /// If null or empty is passed, the current culture will be returned.
        /// </summary>
        /// <param name="culture">The input text.</param>
        /// <returns>A CultureInfo instance.</returns>
        public static CultureInfo ToCultureInfo(string culture)
		{
#if SILVERLIGHT
            return ToCultureInfo(culture, CultureInfo.CurrentCulture);
#else
            return ToCultureInfo(culture, true);
#endif
        }

        /// <summary>
        /// Converts the specified object to an equivalent CultureInfo instance.
        /// The text can be a culture name of the string representation of an lcid.
        /// If null or empty is passed, the current culture will be returned.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="culture">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType(object value, out CultureInfo culture)
        {
            return TryChangeType(value, true, out culture);
        }

        /// <summary>
        /// Converts the specified object to an equivalent CultureInfo instance.
        /// The text can be a culture name of the string representation of an lcid.
        /// If null or empty is passed, the current culture will be returned.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (true) or the default culture settings (false). Not supported on Silverlight</param>
        /// <param name="culture">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType(object value, bool useUserOverride, out CultureInfo culture)
        {
            culture = null;
            if (value == null)
                return false;

            try
            {
#if !SILVERLIGHT && !NETFX_CORE
                int lcid;
                if (TryChangeType(value, out lcid))
                {
                    culture = new CultureInfo(lcid, useUserOverride);
                }
                else
#endif
                {
                    string name;
                    if (!TryChangeType(value, out name))
                        return false;

#if !SILVERLIGHT && !NETFX_CORE
                    culture = new CultureInfo(name, useUserOverride);
#else
                    culture = new CultureInfo(name);
#endif
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the specified object to an equivalent Version instance.
        /// The object can be a version string, or an array of integers.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="version">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType(object value, out Version version)
        {
            version = null;
            if (value == null)
                return false;

            string svalue = value as string;
            if (svalue != null)
            {
                // stupid Version class doesn't know how to handle "4" without a dot
                int i;
                if ((svalue.IndexOf('.') < 0) && (Int32.TryParse(svalue, out i)))
                {
                    svalue += ".0";
                }
                try
                {
                    version = new Version(svalue);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            IEnumerable e = value as IEnumerable;
            if (e != null)
            {
                List<int> ints = new List<int>();
                foreach (object obj in e)
                {
                    int i;
                    if (!TryChangeType(obj, out i))
                        return false;

                    ints.Add(i);
                    if (ints.Count > 3)
                        break;
                }
                if (ints.Count == 0)
                    return false;

                if (ints.Count > 3)
                {
                    version = new Version(ints[0], ints[1], ints[2], ints[3]);
                }
                else if (ints.Count > 2)
                {
                    version = new Version(ints[0], ints[1], ints[2]);
                }
                else if (ints.Count > 1)
                {
                    version = new Version(ints[0], ints[1]);
                }
                else
                {
                    version = new Version(ints[0], 0);
                }
                return true;
            }

            int v;
            if (TryChangeType(value, out v))
            {
                version = new Version(v, 0);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts the specified object to an equivalent Encoding instance.
        /// The value can be an encoding name of the string representation of a code page number.
        /// Not supported on Silverlight.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="encoding">When this method returns, contains the typed value.</param>
        /// <returns>
        /// true if the object was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryChangeType(object value, out Encoding encoding)
        {
            if ((value == null) || (String.Empty.Equals(value)))
            {
#if !SILVERLIGHT && !NETFX_CORE
                encoding = Encoding.Default;
                return true;
#else
                encoding = null;
                return false;
#endif
            }

            encoding = null;
            try
            {
#if !SILVERLIGHT && !NETFX_CORE
                int cp;
                if (TryChangeType(value, out cp))
                {
                    encoding = Encoding.GetEncoding(cp);
                }
                else
#endif
                {
                    string name;
                    if (!TryChangeType(value, out name))
                        return false;

                    encoding = Encoding.GetEncoding(name);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the specified text to an equivalent CultureInfo instance.
        /// The text can be a culture name of the string representation of an lcid.
        /// If empty is passed, the invariant culture will be returned.
        /// If null is passed, the current culture will be returned.
        /// </summary>
        /// <param name="culture">The input text.</param>
        /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (true) or the default culture settings (false).</param>
        /// <returns>A CultureInfo instance.</returns>
        public static CultureInfo ToCultureInfo(string culture, bool useUserOverride)
        {
            return ToCultureInfo(culture, useUserOverride, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the specified text to an equivalent CultureInfo instance.
        /// The text can be a culture name of the string representation of an lcid.
        /// If empty is passed, the invariant culture will be returned.
        /// If null is passed, the current culture will be returned.
        /// </summary>
        /// <param name="culture">The input text.</param>
        /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (true) or the default culture settings (false).</param>
        /// <param name="defaultCulture">The default culture to return if an error occurs.</param>
        /// <returns>A CultureInfo instance.</returns>
        public static CultureInfo ToCultureInfo(string culture, bool useUserOverride, CultureInfo defaultCulture)
		{
            if (String.Empty.Equals(culture))
                return CultureInfo.InvariantCulture;

			if (culture == null)
                return defaultCulture;

#if SILVERLIGHT || NETFX_CORE
            try
            {
                return new CultureInfo(culture);
            }
            catch
            {
                return defaultCulture;
            }
#else
            try
            {
                int lcid;
                if (Int32.TryParse(culture, out lcid))
                    return new CultureInfo(lcid, useUserOverride);
                
                return new CultureInfo(culture, useUserOverride);
            }
            catch
            {
                return defaultCulture;
            }
#endif
        }

        /// <summary>
        /// Formats the specified object.
        /// </summary>
        /// <param name="obj">The object. If null, null will be returned.</param>
        /// <param name="format">The format to use. May be null.</param>
        /// <param name="formatProvider">The format provider. May be null.</param>
        /// <returns>A string containing the value of the current instance in the specified format.</returns>
        public static string Format(object obj, string format, IFormatProvider formatProvider)
        {
            if (obj == null)
                return String.Empty;

            if (String.IsNullOrEmpty(format))
                return obj.ToString();

            if ((format.StartsWith("*")) ||
                (format.StartsWith("#")))
            {
                char sep1 = ' ';
                char sep2 = ':';
                if (format.Length > 1)
                {
                    sep1 = format[1];
                }
                if (format.Length > 2)
                {
                    sep2 = format[2];
                }
                StringBuilder sb = new StringBuilder();
#if NETFX_CORE
                foreach (PropertyInfo pi in obj.GetType().GetRuntimeProperties())
#else
                foreach (PropertyInfo pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
#endif
                {
                    if (!pi.CanRead)
                        continue;
                    
                    if (pi.GetIndexParameters().Length > 0)
                        continue;

                    object value;
                    try
                    {
                        value = pi.GetValue(obj, null);
                    }
                    catch
                    {
                        continue;
                    }
                    if (sb.Length > 0)
                    {
                        if (sep1 != ' ')
                        {
                            sb.Append(sep1);
                        }
                        sb.Append(' ');
                    }
                    if (format[0] == '#')
                    {
                        sb.Append(Decamelize(pi.Name));
                    }
                    else
                    {
                        sb.Append(pi.Name);
                    }
                    sb.Append(sep2);
                    //sb.AppendFormat(formatProvider, "{0}", value);
                    sb.Append(ChangeType<string>(value, String.Format("{0}", value), formatProvider));
                }
                return sb.ToString();
            }
            else if (format.StartsWith("Item[", StringComparison.CurrentCultureIgnoreCase))
            {
                string enumExpression;
                int exprPos = format.IndexOf(']', 5);
                if (exprPos < 0)
                {
                    enumExpression = String.Empty;
                }
                else
                {
                    enumExpression = format.Substring(5, exprPos - 5).Trim();
                    // enumExpression is a lambda like expression with index as the variable
                    // ex: {0: Item[index < 10]} will enum all objects with index < 10
                    // errrhh... so far, since lambda cannot be parsed at runtime, we do nothing...
                }
                IEnumerable enumerable = obj as IEnumerable;
                if (enumerable != null)
                {
                    format = format.Substring(6 + enumExpression.Length);
                    string expression;
                    string separator;
                    if (format.Length == 0)
                    {
                        expression = null;
                        separator = ",";
                    }
                    else
                    {
                        int pos = format.IndexOf(',');
                        if (pos <= 0)
                        {
                            separator = ",";
                            // skip '.'
                            expression = format.Substring(1);
                        }
                        else
                        {
                            separator = format.Substring(pos + 1);
                            expression = format.Substring(1, pos - 1);
                        }
                    }
                    return ConcatenateCollection(enumerable, expression, separator, formatProvider);
                }
            }
            else if (format.IndexOf(',') >= 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string propName in format.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
#if NETFX_CORE
                    PropertyInfo pi = obj.GetType().GetRuntimeProperty(propName);
#else
                    PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
#endif
                    if ((pi == null) || (!pi.CanRead))
                        continue;

                    if (pi.GetIndexParameters().Length > 0)
                        continue;

                    object value;
                    try
                    {
                        value = pi.GetValue(obj, null);
                    }
                    catch
                    {
                        continue;
                    }
                    if (sb.Length > 0)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(pi.Name);
                    sb.Append(':');
                    sb.AppendFormat(formatProvider, "{0}", value);
                }
                return sb.ToString();
            }
            int pos2 = format.IndexOf(':');
            if (pos2 > 0)
            {
                object inner = Evaluate(obj, format.Substring(0, pos2), null);
                if (inner == null)
                    return String.Empty;
                
                return String.Format(formatProvider, "{0:" + format.Substring(pos2 + 1) + "}", inner);
            }
            return (string)Evaluate(obj, format, typeof(string), String.Empty, formatProvider);
        }
	}
}

