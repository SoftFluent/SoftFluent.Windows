
namespace SoftFluent.Windows
{
    /// <summary>
    /// Specifies the comparison operators used by the UniversalConverter.
    /// </summary>
    public enum UniversalConverterOperator
    {
        /// <summary>
        /// A comparison for equality.
        /// </summary>
        Equal,

        /// <summary>
        /// A comparison for inequality.
        /// </summary>
        NotEqual,

        /// <summary>
        /// A comparison for greater than.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// A comparison for greater than or equal to.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// A comparison for lesser than.
        /// </summary>
        LesserThan,

        /// <summary>
        /// A comparison for lesser than or equal to.
        /// </summary>
        LesserThanOrEqual,

        /// <summary>
        /// A range comparison, inclusive on minimum value, exclusive on maximum value.
        /// </summary>
        Between,

        /// <summary>
        /// A comparison between the beginning of this instance and the specified string. This instance is converted to a string if needed before comparison.
        /// </summary>
        StartsWith,

        /// <summary>
        /// A comparison between the beginning of this instance and the specified string. This instance is converted to a string if needed before comparison.
        /// </summary>
        EndsWith,

        /// <summary>
        /// A comparison between this instance content and the specified string. This instance is converted to a string if needed before comparison.
        /// </summary>
        Contains,

        /// <summary>
        /// A comparison for type equality.
        /// </summary>
        IsType,

        /// <summary>
        /// A comparison for type assignability.
        /// </summary>
        IsOfType
    }
}
