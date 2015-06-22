using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Compilation;
using SoftFluent.Windows.Resources;

namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// Represents a set of reflection utilities.
    /// </summary>
	internal sealed class AssemblyUtilities
	{
        private static object _syncObject;
       
        private AssemblyUtilities()
		{
		}

        internal static object SyncObject
        {
            get
            {
                if (_syncObject == null)
                {
                    object obj = new object();
                    Interlocked.CompareExchange(ref _syncObject, obj, null);
                }
                return _syncObject;
            }
        }

        /// <summary>
        /// Defines a parsed type.
        /// </summary>
        public class ParsedType
        {
            private List<ParsedType> _arguments = new List<ParsedType>();
            private string _typeName;
            private static readonly Dictionary<string, ParsedType> _parsedTypes = new Dictionary<string, ParsedType>();

            static ParsedType()
            {
                // add c# shortcuts
                _parsedTypes["string"] = new ParsedType(typeof(string).FullName);
                _parsedTypes["bool"] = new ParsedType(typeof(bool).FullName);
                _parsedTypes["int"] = new ParsedType(typeof(int).FullName);
                _parsedTypes["uint"] = new ParsedType(typeof(uint).FullName);
                _parsedTypes["long"] = new ParsedType(typeof(long).FullName);
                _parsedTypes["ulong"] = new ParsedType(typeof(ulong).FullName);
                _parsedTypes["short"] = new ParsedType(typeof(short).FullName);
                _parsedTypes["ushort"] = new ParsedType(typeof(ushort).FullName);
                _parsedTypes["byte"] = new ParsedType(typeof(byte).FullName);
                _parsedTypes["sbyte"] = new ParsedType(typeof(sbyte).FullName);
                _parsedTypes["float"] = new ParsedType(typeof(float).FullName);
                _parsedTypes["double"] = new ParsedType(typeof(double).FullName);
                _parsedTypes["decimal"] = new ParsedType(typeof(decimal).FullName);
                _parsedTypes["object"] = new ParsedType(typeof(object).FullName);
                _parsedTypes["char"] = new ParsedType(typeof(char).FullName);
            }

            private ParsedType(string typeName)
            {
                if (typeName == null)
                    throw new ArgumentNullException("typeName");

                TypeName = typeName.Trim();
                if ((TypeName.StartsWith("[")) && (TypeName.EndsWith("]")))
                {
                    TypeName = TypeName.Substring(1, TypeName.Length - 2).Trim();
                }

                int asm = TypeName.IndexOf(',');
                if (asm >= 0)
                {
                    AssemblyName = TypeName.Substring(asm + 1).Trim();
                    TypeName = TypeName.Substring(0, asm).Trim();
                }
            }

            /// <summary>
            /// Gets or sets the type name.
            /// </summary>
            /// <value>The type name.</value>
            public string TypeName
            {
                get
                {
                    return _typeName;
                }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");

                    _typeName = value;
                }
            }

            /// <summary>
            /// Gets or sets the assembly name.
            /// </summary>
            /// <value>The assembly name.</value>
            public string AssemblyName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this type is generic.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this type is generic; otherwise, <c>false</c>.
            /// </value>
            public bool IsGeneric { get; set; }

            /// <summary>
            /// Defines output options.
            /// </summary>
            [Flags]
            public enum OutputOptions
            {
                /// <summary>
                /// None.
                /// </summary>
                None = 0,
                
                /// <summary>
                /// Output is C# oriented.
                /// </summary>
                CSharp              = 1,

                /// <summary>
                /// Output is Reflection oriented.
                /// </summary>
                Reflection          = 0x2,
                
                /// <summary>
                /// Output is Visual Basic .NET oriented.
                /// </summary>
                VisualBasic         = 0x4,

                /// <summary>
                /// Output an open type.
                /// </summary>
                Open                = 0x8,

                /// <summary>
                /// Strips assembly names
                /// </summary>
                StripAssemblyNames  = 0x10,

                /// <summary>
                /// Output only the non-generic part.
                /// </summary>
                NonGenericPart      = 0x20
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <param name="options">The options to use.</param>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            public string ToString(OutputOptions options)
            {
                StringBuilder sb = new StringBuilder();
                if (((options & OutputOptions.CSharp) == OutputOptions.CSharp) ||
                    ((options & OutputOptions.VisualBasic) == OutputOptions.VisualBasic))
                {
                    options |= OutputOptions.StripAssemblyNames;
                }
                Append(sb, options);
                return sb.ToString();
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the specified type, using a specific formatting.
            /// </summary>
            /// <param name="type">The type. May no be null.</param>
            /// <param name="options">The options to use.</param>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the specified type.
            /// </returns>
            public static string ToString(Type type, OutputOptions options)
            {
                if (type == null)
                    throw new ArgumentNullException("type");

                ParsedType pt = Parse(type.FullName);
                return pt.ToString(options);
            }

            private void Append(StringBuilder sb, OutputOptions options)
            {
                sb.Append(TypeName);

                if ((IsGeneric) && ((options & OutputOptions.NonGenericPart) != OutputOptions.NonGenericPart))
                {
                    string start;
                    char end;
                    if ((options & OutputOptions.VisualBasic) == OutputOptions.VisualBasic)
                    {
                        start = "(Of ";
                        end = ')';
                    }
                    else
                    {
                        start = "<";
                        end = '>';
                    }

                    if ((options & OutputOptions.Reflection) == OutputOptions.Reflection)
                    {
                        sb.Append('`');
                        sb.Append(_arguments.Count);
                        if ((options & OutputOptions.Open) != OutputOptions.Open)
                        {
                            bool any = false;
                            for (int i = 0; i < _arguments.Count; i++)
                            {
                                if (_arguments[i].TypeName.Length == 0)
                                    continue;

                                if (i > 0)
                                {
                                    sb.Append(',');
                                }
                                else
                                {
                                    sb.Append('[');
                                    any = true;
                                }
                                sb.Append('[');
                                _arguments[i].Append(sb, options);
                                sb.Append(']');
                                if ((i == (_arguments.Count - 1)) && any)
                                {
                                    sb.Append(']');
                                }
                            }
                        }
                    }
                    else
                    {
                        sb.Append(start);
                        for (int i = 0; i < _arguments.Count; i++)
                        {
                            if (i > 0)
                            {
                                sb.Append(',');
                            }
                            if ((options & OutputOptions.Open) != OutputOptions.Open)
                            {
                                _arguments[i].Append(sb, options);
                            }
                        }
                        sb.Append(end);
                    }
                }

                if ((options & OutputOptions.StripAssemblyNames) != OutputOptions.StripAssemblyNames)
                {
                    if (!string.IsNullOrEmpty(AssemblyName))
                    {
                        sb.Append(", ");
                        sb.Append(AssemblyName);
                    }
                }
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            public override string ToString()
            {
                return ToString(OutputOptions.CSharp);
            }

            /// <summary>
            /// Parses the specified type name.
            /// </summary>
            /// <param name="typeName">The type name. May not be null.</param>
            /// <returns>An instance of the parsed type.</returns>
            public static ParsedType Parse(string typeName)
            {
                if (typeName == null)
                    throw new ArgumentNullException("typeName");

                typeName = typeName.Trim();
                if (typeName.StartsWith("Of "))
                {
                    typeName = typeName.Substring(3).Trim();
                }

                lock (SyncObject)
                {
                    ParsedType pt;
                    if (!_parsedTypes.TryGetValue(typeName, out pt))
                    {
                        pt = Parse(typeName, "<", '>');
                        if (pt != null)
                        {
                            _parsedTypes.Add(typeName, pt);
                            return pt;
                        }

                        pt = Parse(typeName, "(Of", ')');
                        if (pt != null)
                        {
                            _parsedTypes.Add(typeName, pt);
                            return pt;
                        }

                        pt = new ParsedType(typeName);
                        _parsedTypes.Add(typeName, pt);
                    }
                    return pt;
                }
            }

            private static ParsedType Parse(string typeName, string start, char end)
            {
                if (typeName == null)
                    throw new ArgumentNullException("typeName");

                if ((typeName.StartsWith("[")) && (typeName.EndsWith("]")))
                {
                    typeName = typeName.Substring(1, typeName.Length - 2).Trim();
                }

                ParsedType pt;
                int lt;
                int gt;
                int quot = typeName.IndexOf('`');
                if (quot >= 0)
                {
                    // reflection style
                    lt = typeName.IndexOf('[', quot + 1);
                    gt = typeName.LastIndexOf(']');
                    if ((lt < 0) || (gt < 0))
                    {
                        int args;
                        int asm = typeName.IndexOf(',', quot);
                        if (asm < 0)
                        {
                            pt = new ParsedType(typeName.Substring(0, quot));
                            args = int.Parse(typeName.Substring(quot + 1));
                        }
                        else
                        {
                            pt = new ParsedType(typeName.Substring(0, quot));
                            pt.AssemblyName = typeName.Substring(asm + 1).Trim();
                            args = int.Parse(typeName.Substring(quot + 1, asm - quot - 1));
                        }
                        for (int i = 0; i < args; i++)
                        {
                            pt._arguments.Add(new ParsedType(string.Empty));
                        }
                        pt.IsGeneric = true;
                        return pt;
                    }
                }
                else
                {
                    lt = typeName.IndexOf(start);
                    gt = typeName.LastIndexOf(end);
                    if ((lt < 0) || (gt < 0))
                        return null;
                }

                if (quot >= 0)
                {
                    pt = new ParsedType(typeName.Substring(0, quot));
                }
                else
                {
                    pt = new ParsedType(typeName.Substring(0, lt));
                }
                pt.IsGeneric = true;

                int startPos = lt + 1;
                int parenCount = 0;
                for (int i = startPos; i < gt; i++)
                {
                    char c = typeName[i];
                    if (parenCount == 0)
                    {
                        if (c == ',')
                        {
                            ParsedType spt = Parse(typeName.Substring(startPos, i - startPos));
                            pt._arguments.Add(spt);
                            startPos = i + 1;
                        }
                        else if (c == '[')
                        {
                            parenCount++;
                        }
                        else if (ChunkStarts(typeName, i, start))
                        {
                            parenCount++;
                            i += start.Length - 1;
                        }
                        else if ((c == end) || (c == ']'))
                        {
                            parenCount--;
                        }
                    }
                    else
                    {
                        if (c == '[')
                        {
                            parenCount++;
                        }
                        else if (ChunkStarts(typeName, i, start))
                        {
                            parenCount++;
                            i += start.Length - 1;
                        }
                        else if ((c == end) || (c == ']'))
                        {
                            parenCount--;
                        }
                    }
                }

                if (parenCount == 0)
                {
                    if (gt < startPos)
                        return null;

                    ParsedType spt = Parse(typeName.Substring(startPos, gt - startPos));
                    pt._arguments.Add(spt);
                }
                return pt;
            }

            private static bool ChunkStarts(string text, int pos, string chunk)
            {
                for (int i = 0; i < chunk.Length; i++)
                {
                    if ((i + pos) > (text.Length - 1))
                        return false;

                    if (text[i + pos] != chunk[i])
                        return false;
                }
                return true;
            }

            /// <summary>
            /// Gets or sets the type arguments.
            /// </summary>
            /// <value>The type arguments.</value>
            public ParsedType[] Arguments
            {
                get
                {
                    return _arguments.ToArray();
                }
                set
                {
                    if (value == null)
                    {
                        _arguments = new List<ParsedType>();
                    }
                    else
                    {
                        _arguments = new List<ParsedType>(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a type from a type name.
        /// The type name can be partially defined. Types in mscorlib.dll and system.dll will be found even when not fully defined.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type.</param>
        /// <returns>The type instance.</returns>
        public static Type GetType(string fullTypeName)
        {
            return GetType(fullTypeName, false);
        }

        /// <summary>
        /// Gets a type from a type name.
        /// The type name can be partially defined. Types in mscorlib.dll and system.dll will be found even when not fully defined.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type.</param>
        /// <param name="throwOnError">if set to <c>true</c> the method will throw on error.</param>
        /// <returns>The type instance.</returns>
        public static Type GetType(string fullTypeName, bool throwOnError)
        {
            return GetType(fullTypeName, throwOnError, null);
        }

        /// <summary>
        /// Gets a type from a type name.
        /// The type name can be partially defined. Types in mscorlib.dll and system.dll will be found even when not fully defined.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type.</param>
        /// <param name="throwOnError">if set to <c>true</c> the method will throw on error.</param>
        /// <param name="hint">A hint assembly if assembly name is not specified. May be null.</param>
        /// <returns>The type instance.</returns>
        public static Type GetType(string fullTypeName, bool throwOnError, Assembly hint)
        {
            if (fullTypeName == null)
                throw new ArgumentNullException("fullTypeName");

            ParsedType pt = ParsedType.Parse(fullTypeName);
            fullTypeName = pt.ToString(ParsedType.OutputOptions.Reflection);
            Type type = null;
            try
            {
                type = Type.GetType(fullTypeName, false);
            }
            catch
            {
                // 1st chance...
            }
            string foundAsm = null;
            if (type == null)
            {
                if (fullTypeName.IndexOf(',') < 0)
                {
                    if (hint != null)
                    {
                        type = hint.GetType(fullTypeName, false);
                    }
                    if (type == null)
                    {
                        // try system
                        // Uri is in system
                        type = Type.GetType(fullTypeName + ", " + typeof(Uri).Assembly.FullName, false);
                    }
                }
                else
                {
                    // try assemblies loaded manually
                    string asm = GetAssemblyName(fullTypeName);
                    if (!string.IsNullOrEmpty(asm))
                    {
                        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if (string.Compare(a.GetName().Name, asm, true) == 0)
                            {
                                foundAsm = a.FullName;
                                string typeName = GetFullName(fullTypeName);
                                type = a.GetType(typeName, false, false);
                            }
                        }
                        if (foundAsm == null)
                        {
                            // try full name
                            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                if (string.Compare(a.FullName, asm, true) == 0)
                                {
                                    foundAsm = a.FullName;
                                    string typeName = GetFullName(fullTypeName);
                                    type = a.GetType(typeName, false, false);
                                }
                            }
                        }
                    }
                }
            }

#if !CLIENT_PROFILE
            if (type == null)
            {
                type = BuildManager.GetType(fullTypeName, false, false);
            }
#endif

            if ((type == null) && (throwOnError))
            {
                try
                {
                    return Type.GetType(fullTypeName, true);
                }
                catch (Exception e)
                {
                    if (foundAsm != null)
                        throw new CodeFluentRuntimeException(SR.GetString("resolveType2", fullTypeName, foundAsm), e);

                    throw new CodeFluentRuntimeException(SR.GetString("resolveType", fullTypeName), e);
                }
            }
            return type;
        }

        /// <summary>
        /// Gets a custom attribute from a collection of attributes
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="attributes">The attributes.</param>
        /// <returns>
        /// An instance of the attribute type if found; otherwise null.
        /// </returns>
        public static T GetAttribute<T>(AttributeCollection attributes) where T: Attribute
        {
            if (attributes == null)
                return null;

            foreach (Attribute att in attributes)
            {
                if (typeof(T).IsAssignableFrom(att.GetType()))
                    return (T)att;
            }
            return null;
        }

        /// <summary>
        /// Gets a custom attribute declared on a member descripor.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="descriptor">The descriptor. May not be null.</param>
        /// <returns>
        /// An instance of the attribute type if found; otherwise null.
        /// </returns>
        public static T GetAttribute<T>(MemberDescriptor descriptor) where T : Attribute
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            return GetAttribute<T>(descriptor.Attributes);
        }

        /// <summary>
        /// Gets a custom attribute declared on custom attribute provider.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="provider">The provider. May not be null.</param>
        /// <returns>
        /// An instance of the attribute type if found; otherwise null.
        /// </returns>
        public static T GetAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            object[] o = provider.GetCustomAttributes(typeof(T), true);
            if ((o == null) || (o.Length == 0))
                return null;

            return (T)o[0];
        }

        /// <summary>
        /// Parses the name of an assembly qualified type name string and returns the full name.
        /// </summary>
        /// <param name="typeName">Full type name to parse. May not be null.</param>
        /// <returns>The type full name. May not be null.</returns>
        public static string GetFullName(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            string name;
            string ns;
            string asm;
            ParseAssemblyQualifiedTypeName(typeName, out name, out ns, out asm);
            return ns + "." + name;
        }

        /// <summary>
        /// Parses the name of an assembly qualified type name string and returns the assembly name.
        /// </summary>
        /// <param name="typeName">Full type name to parse. May not be null.</param>
        /// <returns>The type assembly name. May be null.</returns>
        public static string GetAssemblyName(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            
            string name;
            string ns;
            string asm;
            ParseAssemblyQualifiedTypeName(typeName, out name, out ns, out asm);
            return asm;
        }

        /// <summary>
        /// Parses the name of an assembly qualified type name string.
        /// </summary>
        /// <param name="typeName">Full type name to parse. May not be null.</param>
        /// <param name="name">The type name. May be null.</param>
        /// <param name="namespace">The type namespace. May be null.</param>
        /// <param name="assemblyName">The assembly name. May be null.</param>
		public static void ParseAssemblyQualifiedTypeName(string typeName,
			out string name,
			out string @namespace,
			out string assemblyName)
		{
			if (typeName == null)
				throw new ArgumentNullException("typeName");

            // get to first , skipping < and > (c#) or ( and ) (VB)
            int pos = 0;
            int genericStart = -1;
            bool inGeneric = false;
            while ((pos < typeName.Length))
            {
                if (inGeneric)
                {
                    if ((typeName[pos] == '>') ||   // C#
                        (typeName[pos] == ')'))     // VB
                    {
                        inGeneric = false;
                    }
                }
                else
                {
                    if ((typeName[pos] == '<') ||   // C#
                        (typeName[pos] == '('))     // VB
                    {
                        inGeneric = true;
                        if (genericStart < 0)
                        {
                            genericStart = pos;
                        }
                    }
                    else
                    {
                        if (typeName[pos] == ',')
                            break;
                    }
                }
                pos++;
            }

			string fullTypeName;
			if (pos >= typeName.Length)
			{
				assemblyName = null;
				fullTypeName = typeName;
			}
			else
			{
				assemblyName = ConvertUtilities.Nullify(typeName.Substring(pos + 1), true);
				fullTypeName = ConvertUtilities.Nullify(typeName.Substring(0, pos), true);
			}

            if (fullTypeName == null)
            {
                @namespace = null;
                name = null;
            }
            else
            {
                if (genericStart >= 0)
                {
                    pos = fullTypeName.Substring(0, genericStart).LastIndexOf('.');
                }
                else
                {
                    pos = fullTypeName.LastIndexOf('.');
                }
                if (pos < 0)
                {
                    @namespace = null;
                    name = fullTypeName;
                }
                else
                {
                    name = ConvertUtilities.Nullify(fullTypeName.Substring(pos + 1), true);
                    @namespace = ConvertUtilities.Nullify(fullTypeName.Substring(0, pos), true);
                }
            }
		}

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
        private interface IAssemblyCache
        {
            void Reserved0();

            [PreserveSig]
            int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref AssemblyInfo assemblyInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AssemblyInfo
        {
            public int cbAssemblyInfo; // size of this structure for future expansion
            public int assemblyFlags;
            public long assemblySizeInKB;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string currentAssemblyPath;
            public int cchBuf; // size of path buf.
        }
        
        [ComImport]
        [Guid("6eaf5ace-7917-4f3c-b129-e046a9704766")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IReferenceIdentity
        {
        }

        [ComImport]
        [Guid("261a6983-c35d-4d0d-aa5b-7867259e77bc")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IIdentityAuthority
        {
            int Reserved0();
            int Reserved1();
            int Reserved2();
            int Reserved3();
            int Reserved4();
            int ReferenceToTextBuffer(
                int dwFlags,
                IReferenceIdentity pIReferenceIdentity,
                int cchBufferSize,
                [MarshalAs(UnmanagedType.LPWStr)]
                string buffer,
                ref int pcchBufferRequired);
        };
	}
}
