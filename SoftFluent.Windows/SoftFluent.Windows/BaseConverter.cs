using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using SoftFluent.Windows.Utilities;

namespace SoftFluent.Windows
{
    public class BaseConverter : IConverter
    {
        private static bool NormalizeHexString(ref string s)
        {
            if (s == null)
                return false;

            if (s.Length > 0)
            {
                if (s[0] == 'x' || s[0] == 'X')
                {
                    s = s.Substring(1);
                    return true;
                }

                if (s.Length > 1)
                {
                    if (s[0] == '0' && (s[1] == 'x' || s[1] == 'X'))
                    {
                        s = s.Substring(2);
                        return true;
                    }
                }
            }
            return false;
        }

        private static void GetBytes(decimal d, byte[] buffer)
        {
            var ints = decimal.GetBits(d);
            buffer[0] = (byte)ints[0];
            buffer[1] = (byte)(ints[0] >> 8);
            buffer[2] = (byte)(ints[0] >> 0x10);
            buffer[3] = (byte)(ints[0] >> 0x18);
            buffer[4] = (byte)ints[1];
            buffer[5] = (byte)(ints[1] >> 8);
            buffer[6] = (byte)(ints[1] >> 0x10);
            buffer[7] = (byte)(ints[1] >> 0x18);
            buffer[8] = (byte)ints[2];
            buffer[9] = (byte)(ints[2] >> 8);
            buffer[10] = (byte)(ints[2] >> 0x10);
            buffer[11] = (byte)(ints[2] >> 0x18);
            buffer[12] = (byte)ints[3];
            buffer[13] = (byte)(ints[3] >> 8);
            buffer[14] = (byte)(ints[3] >> 0x10);
            buffer[15] = (byte)(ints[3] >> 0x18);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out DateTimeOffset value)
        {
            if (DateTimeOffset.TryParse(Convert.ToString(input, provider), provider, DateTimeStyles.None, out value))
                return true;

            DateTimeOffset? nb = input as DateTimeOffset?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            DateTime dt;
            if (TryConvert(input, provider, out dt))
            {
                value = new DateTimeOffset(dt);
                return true;
            }
            value = DateTimeOffset.MinValue;
            return false;
        }

        private static bool TryConvert(object input, IFormatProvider provider, out TimeSpan value)
        {
            if (TimeSpan.TryParse(Convert.ToString(input, provider), provider, out value))
                return true;

            TimeSpan? nb = input as TimeSpan?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            long l;
            if (TryConvert(input, provider, out l))
            {
                value = new TimeSpan(l);
                return true;
            }
            value = TimeSpan.Zero;
            return false;
        }

        private static bool TryConvert(object input, IFormatProvider provider, out IntPtr value)
        {
            IntPtr? nb = input as IntPtr?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = IntPtr.Zero;
            if (IntPtr.Size == 4)
            {
                int i;
                if (TryConvert(input, provider, out i))
                {
                    value = new IntPtr(i);
                    return true;
                }
                return false;
            }

            long l;
            if (TryConvert(input, provider, out l))
            {
                value = new IntPtr(l);
                return true;
            }
            return false;
        }

        private static bool TryConvert(object input, IFormatProvider provider, out Guid value)
        {
            Guid? nb = input as Guid?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            byte[] inputBytes = input as byte[];
            if (inputBytes != null)
            {
                if (inputBytes.Length != 16)
                {
                    value = Guid.Empty;
                    return false;
                }

                value = new Guid(inputBytes);
                return true;
            }

            return Guid.TryParse(Convert.ToString(input, provider), out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out ulong value)
        {
            ulong? nb = input as ulong?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToUInt64(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return ulong.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out ushort value)
        {
            ushort? nb = input as ushort?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToUInt16(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return ushort.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out decimal value)
        {
            decimal? nb = input as decimal?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToDecimal(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return decimal.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out float value)
        {
            float? nb = input as float?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToSingle(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return float.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out double value)
        {
            double? nb = input as double?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToDouble(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return double.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out char value)
        {
            value = '\0';
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToChar(provider);
                    return true;
                }
                catch
                {
                }
            }

            string s = Convert.ToString(input, provider);
            return char.TryParse(s, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out DateTime value)
        {
            DateTime? nb = input as DateTime?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = DateTime.MinValue;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToDateTime(provider);
                    return true;
                }
                catch
                {
                }
            }

            DateTimeStyles styles = DateTimeStyles.None;
            string s = Convert.ToString(input, provider);
            return DateTime.TryParse(s, provider, styles, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out uint value)
        {
            uint? nb = input as uint?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToUInt32(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return uint.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out byte value)
        {
            byte? nb = input as byte?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToByte(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return byte.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out sbyte value)
        {
            sbyte? nb = input as sbyte?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToSByte(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return sbyte.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out short value)
        {
            short? nb = input as short?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            byte[] inputBytes = input as byte[];
            if (inputBytes != null)
            {
                if (inputBytes.Length == 2)
                {
                    value = BitConverter.ToInt16(inputBytes, 0);
                    return true;
                }
                return false;
            }

            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToInt16(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return short.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out int value)
        {
            int? nb = input as int?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            byte[] inputBytes = input as byte[];
            if (inputBytes != null)
            {
                if (inputBytes.Length == 4)
                {
                    value = BitConverter.ToInt32(inputBytes, 0);
                    return true;
                }
                return false;
            }

            if (input is IntPtr)
            {
                value = ((IntPtr)input).ToInt32();
                return true;
            }

            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToInt32(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return int.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out long value)
        {
            long? nb = input as long?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = 0;
            byte[] inputBytes = input as byte[];
            if (inputBytes != null)
            {
                if (inputBytes.Length == 8)
                {
                    value = BitConverter.ToInt64(inputBytes, 0);
                    return true;
                }
                return false;
            }

            if (input is IntPtr)
            {
                value = ((IntPtr)input).ToInt64();
                return true;
            }

            IConvertible ic = input as IConvertible;
            if (ic != null)
            {
                try
                {
                    value = ic.ToInt64(provider);
                    return true;
                }
                catch
                {
                }
            }

            NumberStyles styles = NumberStyles.Integer;
            string s = Convert.ToString(input, provider);
            if (NormalizeHexString(ref s))
            {
                styles |= NumberStyles.AllowHexSpecifier;
            }
            return long.TryParse(s, styles, provider, out value);
        }

        private static bool TryConvert(object input, IFormatProvider provider, out bool value)
        {
            bool? nb = input as bool?;
            if (nb != null)
            {
                value = nb.Value;
                return true;
            }

            value = false;
            byte[] inputBytes = input as byte[];
            if (inputBytes != null)
            {
                if (inputBytes.Length == 1)
                {
                    value = BitConverter.ToBoolean(inputBytes, 0);
                    return true;
                }
                return false;
            }

            object booll;
            if (TryConvert(input, typeof(long), provider, out booll))
            {
                value = ((long)booll) != 0;
                return true;
            }

            string bools = Convert.ToString(input, provider);
            if (bools == null)
                return false; // arguable...

            bools = bools.Trim().ToLowerInvariant();
            if (bools == "y" || bools == "yes" || bools == "t" || bools.StartsWith("true"))
            {
                value = true;
                return true;
            }

            if (bools == "n" || bools == "no" || bools == "f" || bools.StartsWith("false"))
                return true;

            return false;
        }

        private static MethodInfo _enumTryParse = typeof(Enum).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "TryParse" && m.GetParameters().Length == 3)
            .First();

        private static bool EnumTryParse(Type type, string input, out object value)
        {
            MethodInfo mi = _enumTryParse.MakeGenericMethod(type);
            object[] args = new [] { input, true, Enum.ToObject(type, 0) };
            bool b = (bool)mi.Invoke(null, args);
            value = args[2];
            return b;
        }

        public virtual bool TryChangeType(object input, Type conversionType, IFormatProvider provider, out object value)
        {
            return TryConvert(input, conversionType, provider, out value);
        }

        public static bool TryConvert(object input, Type conversionType, IFormatProvider provider, out object value)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (input == null)
            {
                if (conversionType.IsValueType)
                {
                    value = Activator.CreateInstance(conversionType);
                    return false;
                }

                value = null;
                return true;
            }

            Type inputType = input.GetType();
            TypeCode inputCode = Type.GetTypeCode(inputType);
            TypeCode conversionCode = Type.GetTypeCode(conversionType);
            if (conversionType.IsAssignableFrom(inputType))
            {
                value = input;
                return true;
            }

            if (conversionType.IsNullable())
            {
                object vtValue;
                Type vtType = conversionType.GetGenericArguments()[0];
                if (TryConvert(input, vtType, provider, out vtValue))
                {
                    Type nt = typeof(Nullable<>).MakeGenericType(vtType);
                    value = Activator.CreateInstance(nt, vtValue);
                    return true;
                }
                value = null;
                return false;
            }

            if (Convert.IsDBNull(input))
            {
                if (conversionType.IsValueType)
                {
                    value = Activator.CreateInstance(conversionType);
                    return false;
                }

                value = null;
                return true;
            }

            // enum must be before integers
            if (conversionType.IsEnum)
            {
                if (EnumTryParse(conversionType, Convert.ToString(input, provider), out value))
                    return true;
            }

            switch (conversionCode)
            {
                case TypeCode.Boolean:
                    bool boolValue;
                    if (TryConvert(input, provider, out boolValue))
                    {
                        value = boolValue;
                        return true;
                    }
                    break;

                case TypeCode.Byte:
                    byte byteValue;
                    if (TryConvert(input, provider, out byteValue))
                    {
                        value = byteValue;
                        return true;
                    }
                    break;

                case TypeCode.Char:
                    char charValue;
                    if (TryConvert(input, provider, out charValue))
                    {
                        value = charValue;
                        return true;
                    }
                    break;

                case TypeCode.DateTime:
                    DateTime dtValue;
                    if (TryConvert(input, provider, out dtValue))
                    {
                        value = dtValue;
                        return true;
                    }
                    break;

                case TypeCode.DBNull:
                    value = input == null ? Convert.DBNull : null;
                    return input == null;

                case TypeCode.Decimal:
                    decimal decValue;
                    if (TryConvert(input, provider, out decValue))
                    {
                        value = decValue;
                        return true;
                    }
                    break;

                case TypeCode.Double:
                    double dblValue;
                    if (TryConvert(input, provider, out dblValue))
                    {
                        value = dblValue;
                        return true;
                    }
                    break;

                case TypeCode.Int16:
                    short i16Value;
                    if (TryConvert(input, provider, out i16Value))
                    {
                        value = i16Value;
                        return true;
                    }
                    break;

                case TypeCode.Int32:
                    int i32Value;
                    if (TryConvert(input, provider, out i32Value))
                    {
                        value = i32Value;
                        return true;
                    }
                    break;

                case TypeCode.Int64:
                    long i64Value;
                    if (TryConvert(input, provider, out i64Value))
                    {
                        value = i64Value;
                        return true;
                    }
                    break;

                case TypeCode.SByte:
                    sbyte sbyteValue;
                    if (TryConvert(input, provider, out sbyteValue))
                    {
                        value = sbyteValue;
                        return true;
                    }
                    break;

                case TypeCode.Single:
                    float fltValue;
                    if (TryConvert(input, provider, out fltValue))
                    {
                        value = fltValue;
                        return true;
                    }
                    break;

                case TypeCode.String:
                    value = Convert.ToString(input, provider);
                    return true;

                case TypeCode.UInt16:
                    ushort u16Value;
                    if (TryConvert(input, provider, out u16Value))
                    {
                        value = u16Value;
                        return true;
                    }
                    break;

                case TypeCode.UInt32:
                    uint u32Value;
                    if (TryConvert(input, provider, out u32Value))
                    {
                        value = u32Value;
                        return true;
                    }
                    break;

                case TypeCode.UInt64:
                    ulong u64Value;
                    if (TryConvert(input, provider, out u64Value))
                    {
                        value = u64Value;
                        return true;
                    }
                    break;

                case TypeCode.Object:
                    if (conversionType == typeof(Guid))
                    {
                        Guid gValue;
                        if (TryConvert(input, provider, out gValue))
                        {
                            value = gValue;
                            return true;
                        }
                    }

                    if (conversionType == typeof(IntPtr))
                    {
                        IntPtr ptr;
                        if (TryConvert(input, provider, out ptr))
                        {
                            value = ptr;
                            return true;
                        }
                    }

                    if (conversionType == typeof(Version))
                    {
                        Version version;
                        if (Version.TryParse(Convert.ToString(input, provider), out version))
                        {
                            value = version;
                            return true;
                        }
                    }

                    if (conversionType == typeof(IPAddress))
                    {
                        IPAddress address;
                        if (IPAddress.TryParse(Convert.ToString(input, provider), out address))
                        {
                            value = address;
                            return true;
                        }
                    }

                    if (conversionType == typeof(DateTimeOffset))
                    {
                        DateTimeOffset dto;
                        if (TryConvert(input, provider, out dto))
                        {
                            value = dto;
                            return true;
                        }
                    }

                    if (conversionType == typeof(TimeSpan))
                    {
                        TimeSpan ts;
                        if (TryConvert(input, provider, out ts))
                        {
                            value = ts;
                            return true;
                        }
                    }

                    if (conversionType == typeof(byte[]))
                    {
                        switch (inputCode)
                        {
                            case TypeCode.Boolean:
                                value = BitConverter.GetBytes((bool)input);
                                return true;

                            case TypeCode.Char:
                                value = BitConverter.GetBytes((char)input);
                                return true;

                            case TypeCode.Double:
                                value = BitConverter.GetBytes((double)input);
                                return true;

                            case TypeCode.Int16:
                                value = BitConverter.GetBytes((short)input);
                                return true;

                            case TypeCode.Int32:
                                value = BitConverter.GetBytes((int)input);
                                return true;

                            case TypeCode.Int64:
                                value = BitConverter.GetBytes((long)input);
                                return true;

                            case TypeCode.Single:
                                value = BitConverter.GetBytes((float)input);
                                return true;

                            case TypeCode.UInt16:
                                value = BitConverter.GetBytes((ushort)input);
                                return true;

                            case TypeCode.UInt32:
                                value = BitConverter.GetBytes((uint)input);
                                return true;

                            case TypeCode.UInt64:
                                value = BitConverter.GetBytes((ulong)input);
                                return true;

                            case TypeCode.Byte:
                                value = new byte[] { (byte)input };
                                return true;

                            case TypeCode.DateTime:
                                value = BitConverter.GetBytes(((DateTime)input).ToOADate());
                                return true;

                            case TypeCode.Decimal:
                                var decBytes = new byte[16];
                                GetBytes((decimal)input, decBytes);
                                value = decBytes;
                                return true;

                            case TypeCode.SByte:
                                value = new byte[] { unchecked((byte)input) };
                                return true;

                            case TypeCode.String:
                                value = Convert.FromBase64String((string)input);
                                return true;

                            default:
                                if (input is Guid)
                                {
                                    value = ((Guid)input).ToByteArray();
                                    return true;
                                }

                                if (input is DateTimeOffset)
                                    return TryConvert(((DateTimeOffset)input).DateTime, conversionType, provider, out value);

                                if (input is TimeSpan)
                                {
                                    value = BitConverter.GetBytes(((TimeSpan)input).Ticks);
                                    return true;
                                }
                                break;
                        }
                    }
                    break;
            }

            TypeConverter ctConverter = null;
            try
            {
                ctConverter = TypeDescriptor.GetConverter(conversionType);
                if (ctConverter != null && ctConverter.CanConvertFrom(inputType))
                {
                    value = ctConverter.ConvertFrom(null, provider as CultureInfo, input);
                    return true;
                }
            }
            catch
            {
                // do nothing
            }

            try
            {
                TypeConverter inputConverter = TypeDescriptor.GetConverter(inputType);
                if (inputConverter != null && inputConverter.CanConvertTo(conversionType))
                {
                    value = inputConverter.ConvertTo(null, provider as CultureInfo, input, conversionType);
                    return true;
                }
            }
            catch
            {
                // do nothing
            }

            // call a possible TryParse method
            object defaultValue = conversionType.IsValueType ? conversionType.Assembly.CreateInstance(conversionType.FullName) : null;
            MethodInfo mi = conversionType.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), conversionType.MakeByRefType() }, null);
            if (mi != null && mi.ReturnType == typeof(bool))
            {
                object refValue = defaultValue;
                object[] parameters = new object[] { Convert.ToString(input, provider), refValue };
                bool b = (bool)mi.Invoke(null, parameters);
                value = parameters[1];
                return b;
            }

            try
            {
                if (ctConverter != null && !(input is string) && ctConverter.CanConvertFrom(typeof(string)))
                {
                    value = ctConverter.ConvertFrom(null, provider as CultureInfo, Convert.ToString(input, provider));
                    return true;
                }
            }
            catch
            {
                // do nothing
            }

            value = defaultValue;
            return false;
        }
    }
}
