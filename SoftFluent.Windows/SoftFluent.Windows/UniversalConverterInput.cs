using System;

namespace SoftFluent.Windows
{
    /// <summary>
    /// Defines input for a comparison test.
    /// </summary>
    public class UniversalConverterInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalConverterInput"/> class.
        /// </summary>
        public UniversalConverterInput()
        {
            Options = UniversalConverterOptions.Convert;
        }

        /// <summary>
        /// Gets or sets the value to test.
        /// </summary>
        /// <value>
        /// The value to test.
        /// </value>
        public virtual object Value { get; set; }

        /// <summary>
        /// Gets or sets the value to use for general comparison.
        /// </summary>
        /// <value>
        /// The value to test.
        /// </value>
        public virtual object ValueToCompare { get; set; }

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
        /// Gets or sets the converted parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public virtual object ConverterParameter { get; set; }

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
        /// Gets or sets a value indicating whether the result must be reversed.
        /// </summary>
        /// <value>true if the result must be reversed; false otherwise.</value>
        public virtual bool Reverse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how to compare strings.
        /// </summary>
        /// <value>
        /// The string comparison.
        /// </value>
        public virtual StringComparison StringComparison { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public virtual UniversalConverterInput Clone()
        {
            UniversalConverterInput clone = new UniversalConverterInput();
            clone.MaximumValue = MaximumValue;
            clone.MinimumValue = MinimumValue;
            clone.Operator = Operator;
            clone.Options = Options;
            clone.Value = Value;
            clone.ValueToCompare = ValueToCompare;
            clone.Reverse = Reverse;
            clone.StringComparison = StringComparison;
            return clone;
        }

        private Type ValueToCompareToType(IFormatProvider provider)
        {
            Type type = Value as Type;
            if (type != null)
                return type;

            string name = ValueToCompareToString(provider, true);
            if (string.IsNullOrEmpty(name))
                return null;

            return TypeResolutionService.ResolveType(name);
        }

        private string ValueToCompareToString(IFormatProvider provider, bool forceConvert)
        {
            if (ValueToCompare == null)
                return null;

            string v = ValueToCompare as string;
            if (v == null)
            {
                if (forceConvert || (Options & UniversalConverterOptions.Convert) == UniversalConverterOptions.Convert)
                {
                    v = ConversionService.ChangeType<string>(ValueToCompare, null, provider);
                    if (v == null)
                    {
                        v = string.Format(provider, "{0}", ValueToCompare);
                    }
                }
            }

            if ((Options & UniversalConverterOptions.Trim) == UniversalConverterOptions.Trim)
            {
                if (v != null)
                {
                    v = v.Trim();
                }
            }

            if ((Options & UniversalConverterOptions.Nullify) == UniversalConverterOptions.Nullify)
            {
                if (v != null && v.Length == 0)
                {
                    v = null;
                }
            }
            return v;
        }

        private string ValueToString(IFormatProvider provider)
        {
            if (Value == null)
                return null;

            string v = Value as string;
            if (v == null)
            {
                v = ConversionService.ChangeType<string>(Value, null, provider);
                if (v == null)
                {
                    v = string.Format(provider, "{0}", Value);
                }
            }
            return v;
        }

        /// <summary>
        /// Determine if the input values matches, depending on the input operator and options.
        /// </summary>
        /// <returns>true if the values matches; false otherwise.</returns>
        public bool Matches()
        {
            return Matches(null);
        }

        /// <summary>
        /// Determine if the input values matches, depending on the input operator and options.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>true if the values matches; false otherwise.</returns>
        public virtual bool Matches(IFormatProvider provider)
        {
            bool ret = false;
            string v;
            string vtc;
            UniversalConverterInput clone;
            switch (Operator)
            {
                case UniversalConverterOperator.Equal:
                    if (Value == null)
                    {
                        if (ValueToCompare == null)
                        {
                            ret = true;
                            break;
                        }

                        // use string comparison (to compare "" to null or similar)
                        v = ValueToCompareToString(provider, false);
                        ret = v == null;
                        break; // false
                    }

                    if (Value.Equals(ValueToCompare))
                    {
                        ret = true;
                        break;
                    }

                    v = Value as string;
                    if (v != null)
                    {
                        vtc = ValueToCompareToString(provider, true);
                        ret = string.Compare(v, vtc, StringComparison) == 0;
                        break;
                    }

                    if ((Options & UniversalConverterOptions.Convert) == UniversalConverterOptions.Convert)
                    {
                        object cvalue;
                        if (ConversionService.TryChangeType(ValueToCompare, Value.GetType(), provider, out cvalue))
                        {
                            if (Value.Equals(cvalue))
                            {
                                ret = true;
                                break;
                            }

                            if (Value.GetType() == typeof(string))
                            {
                                string sv = (string)cvalue;
                                if ((Options & UniversalConverterOptions.Trim) == UniversalConverterOptions.Trim)
                                {
                                    sv = sv.Trim();
                                }

                                if ((Options & UniversalConverterOptions.Nullify) == UniversalConverterOptions.Nullify)
                                {
                                    if (sv.Length == 0)
                                    {
                                        sv = null;
                                    }
                                }

                                if (Value.Equals(sv))
                                {
                                    ret = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case UniversalConverterOperator.NotEqual:
                    clone = Clone();
                    clone.Operator = UniversalConverterOperator.Equal;
                    ret = clone.Matches(provider);
                    break;

                case UniversalConverterOperator.Contains:
                    v = ValueToString(provider);
                    if (v == null)
                        break;

                    vtc = ValueToCompareToString(provider, true);
                    if (vtc == null)
                        break;

                    ret = v.IndexOf(vtc, StringComparison) >= 0;
                    break;

                case UniversalConverterOperator.StartsWith:
                    v = ValueToString(provider);
                    if (v == null)
                        break;

                    vtc = ValueToCompareToString(provider, true);
                    if (vtc == null)
                        break;

                    ret = v.StartsWith(vtc, StringComparison);
                    break;

                case UniversalConverterOperator.EndsWith:
                    v = ValueToString(provider);
                    if (v == null)
                        break;

                    vtc = ValueToCompareToString(provider, true);
                    if (vtc == null)
                        break;

                    ret = v.EndsWith(vtc, StringComparison);
                    break;

                case UniversalConverterOperator.LesserThanOrEqual:
                case UniversalConverterOperator.LesserThan:
                case UniversalConverterOperator.GreaterThanOrEqual:
                case UniversalConverterOperator.GreaterThan:
                    IComparable cv = Value as IComparable;
                    if (cv == null || ValueToCompare == null)
                        break;

                    IComparable cvtc;
                    if (!Value.GetType().IsAssignableFrom(ValueToCompare.GetType()))
                    {
                        cvtc = ConversionService.ChangeType(ValueToCompare, Value.GetType(), provider) as IComparable;
                    }
                    else
                    {
                        cvtc = ValueToCompare as IComparable;
                    }
                    if (cvtc == null)
                        break;

                    int comparison;
                    v = Value as string;
                    if (StringComparison != StringComparison.CurrentCulture) // not the default? use the special string comparer
                    {
                        vtc = ValueToCompareToString(provider, true);
                        comparison = string.Compare(v, vtc, StringComparison);
                    }
                    else
                    {
                        comparison = cv.CompareTo(cvtc);
                    }

                    if (comparison == 0)
                    {
                        ret = Operator == UniversalConverterOperator.GreaterThanOrEqual || Operator == UniversalConverterOperator.LesserThanOrEqual;
                        break;
                    }

                    if (comparison < 0)
                    {
                        ret = Operator == UniversalConverterOperator.LesserThan || Operator == UniversalConverterOperator.LesserThanOrEqual;
                        break;
                    }

                    ret = Operator == UniversalConverterOperator.GreaterThan || Operator == UniversalConverterOperator.GreaterThanOrEqual;
                    break;

                case UniversalConverterOperator.Between:
                    clone = Clone();
                    clone.ValueToCompare = MinimumValue;
                    clone.Operator = UniversalConverterOperator.GreaterThanOrEqual;
                    if (!clone.Matches(provider))
                        break;

                    clone = Clone();
                    clone.ValueToCompare = MaximumValue;
                    clone.Operator = UniversalConverterOperator.LesserThan;
                    ret = clone.Matches(provider);
                    break;

                case UniversalConverterOperator.IsType:
                case UniversalConverterOperator.IsOfType:
                    Type tvtc = ValueToCompareToType(provider);
                    if (tvtc == null)
                        break;

                    if (Value == null)
                    {
                        if (tvtc.IsValueType)
                            break;

                        ret = (Options & UniversalConverterOptions.NullMatchesType) == UniversalConverterOptions.NullMatchesType;
                        break;
                    }

                    if (Operator == UniversalConverterOperator.IsType)
                    {
                        ret = Value.GetType() == tvtc;
                    }
                    else
                    {
                        ret = tvtc.IsAssignableFrom(Value.GetType());
                    }
                    break;
            }

            return Reverse ? !ret : ret;
        }
    }
}
