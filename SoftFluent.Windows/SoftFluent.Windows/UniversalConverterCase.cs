using System;
using System.Globalization;

namespace SoftFluent.Windows
{
    /// <summary>
    /// Defines a case for the UniversalConverter class.
    /// </summary>
    public class UniversalConverterCase
    {
        /// <summary>
        /// Gets or sets the value to test.
        /// </summary>
        /// <value>
        /// The value to test.
        /// </value>
        public virtual object Value { get; set; }

        /// <summary>
        /// Gets or sets the converted value to return if there is a match.
        /// </summary>
        /// <value>
        /// The converted value to return.
        /// </value>
        public virtual object ConvertedValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum value to use for range comparison.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public virtual object MinimumValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value to use for range comparison.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public virtual object MaximumValue { get; set; }

        /// <summary>
        /// Gets or sets the options to use to determine if there is a match.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public virtual UniversalConverterOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the operator to use to determine if there is a match. The default value is Equal.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public virtual UniversalConverterOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how to compare strings.
        /// </summary>
        /// <value>
        /// The string comparison.
        /// </value>
        public virtual StringComparison StringComparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the result must be reversed.
        /// </summary>
        /// <value>true if the result must be reversed; false otherwise.</value>
        public virtual bool Reverse { get; set; }

        /// <summary>
        /// Checks Value with the specified input value.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="parameter">The parameter passed from the value converter.</param>
        /// <param name="culture">The culture passed from the value converter.</param>
        /// <returns>true if Value is equals to the specified value.</returns>
        public virtual bool Matches(object value, object parameter, CultureInfo culture)
        {
            UniversalConverterInput input = new UniversalConverterInput();
            input.MaximumValue = MaximumValue;
            input.MinimumValue = MinimumValue;
            input.Operator = Operator;
            input.Options = Options;
            input.Value = Value;
            input.ValueToCompare = value;
            input.Reverse = Reverse;
            input.StringComparison = StringComparison;
            input.ConverterParameter = parameter;
            return input.Matches(culture);
        }
    }
}
