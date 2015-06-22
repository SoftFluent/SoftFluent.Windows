using System;

namespace SoftFluent.Windows
{
    /// <summary>
    /// Defines validation options for the UniversalConverter.
    /// </summary>
    [Flags]
    public enum UniversalConverterOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Determines if string values must be trimmed before comparisons.
        /// </summary>
        Trim = 0x1,

        /// <summary>
        /// Determines if values must be converted before final comparisons.
        /// </summary>
        Convert = 0x2,

        /// <summary>
        /// Determines if string values must be nullified before comparisons.
        /// </summary>
        Nullify = 0x4,

        /// <summary>
        /// Determines if a null value matches a type check (if the type is a reference type).
        /// </summary>
        NullMatchesType = 0x8,

#if !NETFX_CORE && !SILVERLIGHT
        /// <summary>
        /// Determines if javascript expression may throw errors.
        /// </summary>
        DontThrowJavascriptErrors = 0x10,
#endif

        /// <summary>
        /// Use the converter parameter as the converted value.
        /// </summary>
        ConvertedValueIsConverterParameter = 0x20,
    }
}
