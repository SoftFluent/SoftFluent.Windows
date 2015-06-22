using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SoftFluent.Windows.Resources;

namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// The exception that is thrown when an entity state is not valid.
    /// </summary>
#if SILVERLIGHT || NETFX_CORE
    // ISerializable, IDeserializationCallback, SerializableAttribute, etc. not available with Silverlight. declaring it in the CF runtime will have no meaning.
#else
	[Serializable]
#endif
    public class CodeFluentValidationException : CodeFluentRuntimeException
	{
	    /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentValidationException"/> class.
        /// </summary>
		public CodeFluentValidationException()
			:base(SR.GetString("codeFluentValidationException"))
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
		public CodeFluentValidationException(string message)
			:base(message)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="results">A list of exceptions.</param>
        public CodeFluentValidationException(string message, IList<CodeFluentValidationException> results)
            : base(message)
        {
            Results = results;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
		public CodeFluentValidationException(string message, Exception innerException)
			:base(message, innerException)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">Name of the member that failed validation. May not be null.</param>
        public CodeFluentValidationException(string message, string memberName)
            : base(message)
        {
            if (memberName == null)
                throw new ArgumentNullException("memberName");

            MemberName = memberName;
        }

#if SILVERLIGHT || NETFX_CORE
        // this constructor is not available in silverlight
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentValidationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		protected CodeFluentValidationException(SerializationInfo info, StreamingContext context)
			:base(info, context)
		{
		}
#endif

	    /// <summary>
	    /// Gets a list of validation exceptions. May be null.
	    /// </summary>
	    /// <value>The results.</value>
	    public IList<CodeFluentValidationException> Results { get; private set; }

	    /// <summary>
	    /// Gets the name of the member that failed the validation.
	    /// </summary>
	    /// <value>The name of the member. May be null.</value>
	    public string MemberName { get; private set; }
	}
}

