using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using SoftFluent.Windows.Resources;

namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// The exception that is thrown when a generic CodeFluent runtime error occurs.
    /// </summary>
#if SILVERLIGHT || NETFX_CORE
#else
	[Serializable]
#endif
    public class CodeFluentRuntimeException : Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentRuntimeException"/> class.
        /// </summary>
		public CodeFluentRuntimeException()
			:base(SR.GetString("codeFluentRuntimeException"))
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentRuntimeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
		public CodeFluentRuntimeException(string message)
			:base(message)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentRuntimeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
		public CodeFluentRuntimeException(string message, Exception innerException)
			:base(message, innerException)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentRuntimeException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
		public CodeFluentRuntimeException(Exception innerException)
			:base(null, innerException)
		{
		}

#if !CLIENT_PROFILE && !NETFX_CORE

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentRuntimeException"/> class.
        /// </summary>
        /// <param name="type">The type of exception.</param>
        /// <param name="args">An array of optional arguments.</param>
        public CodeFluentRuntimeException(UserExceptionType type, params object[] args)
            : base(GetUserMessage(null, type, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentRuntimeException"/> class.
        /// </summary>
        /// <param name="type">The type of exception.</param>
        /// <param name="args">An array of optional arguments.</param>
        /// <param name="innerException">The inner exception.</param>
        public CodeFluentRuntimeException(UserExceptionType type, Exception innerException, params object[] args)
            : base(GetUserMessage(null, type, args), innerException)
        {
        }

        /// <summary>
        /// Defines the prefix for User Exception resource messages.
        /// Currently defined as "UEM".
        /// </summary>
        public const string UserMessagePrefix = "UEM";

        /// <summary>
        /// Gets a standard message for the specified exception type.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="type">The type of exception.</param>
        /// <param name="args">An array of optional arguments.</param>
        /// <returns>A text describing the exception.</returns>
        public static string GetUserMessage(CultureInfo culture, UserExceptionType type, params object[] args)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            string name = UserMessagePrefix + "." + type;
            return SR.GetString(culture, name, args);
        }
#endif

#if SILVERLIGHT || NETFX_CORE
		// this constructor is not available in silverlight
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFluentRuntimeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		protected CodeFluentRuntimeException(SerializationInfo info, StreamingContext context)
			:base(info, context)
		{
		}
#endif

        /// <summary>
        /// Throws an Exception with the specified name.
        /// </summary>
        /// <param name="name">The name to use. May not be null.</param>
        /// <param name="arguments">The arguments to pass.</param>
		public static void Throw(string name, params object[] arguments)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			throw new CodeFluentRuntimeException(SR.GetString(name, arguments));
		}

        /// <summary>
        /// Gets the message without the code, if it exists. If there is no CodeFluent code, the original message is returned.
        /// </summary>
        /// <value>The message without the code.</value>
        public string MessageWithoutCode
        {
            get
            {
                if (Message == null)
                    return null;

                if (!Message.StartsWith("CF"))
                    return Message;

                int pos = Message.IndexOf(':', 2);
                if (pos < 0)
                    return Message;

                return Message.Substring(pos + 1).TrimStart();
            }
        }

        /// <summary>
        /// Gets the code from a CodeFluent exception message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static int GetCode(string message)
        {
            if (message == null)
                return 0;

            if (!message.StartsWith("CF"))
                return 0;

            int pos = message.IndexOf(':', 2);
            if (pos < 0)
                return 0;

            return ConvertUtilities.ToInt32(message.Substring(2, pos - 2), 0);
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public int Code
        {
            get
            {
                return GetCode(Message);
            }
        }

        /// <summary>
        /// Gets all messages of an exception, including inner exceptions message.
        /// </summary>
        /// <param name="exception">The exception. May be null.</param>
        /// <returns>A newline concatenated list of messages.</returns>
        public static string GetAllMessages(Exception exception)
        {
            return GetAllMessages(exception, Environment.NewLine);
        }

        /// <summary>
        /// Gets all messages of an exception, including inner exceptions message.
        /// </summary>
        /// <param name="exception">The exception. May be null.</param>
        /// <param name="separator">The separator to use. May be null.</param>
        /// <returns>A concatenated list of messages.</returns>
        public static string GetAllMessages(Exception exception, string separator)
        {
            if (exception == null)
                return null;

            StringBuilder sb = new StringBuilder();
            AppendMessages(sb, exception, separator);
            return sb.ToString().Replace("..", ".");
        }

        /// <summary>
        /// Gets the interesting exception from an exception. This methods removes the leading TargetInvocationExceptions.
        /// </summary>
        /// <param name="exception">The original exception. May be null.</param>
        /// <returns>An exception instance.</returns>
        public static Exception GetInterestingException(Exception exception)
        {
            if (exception == null)
                return null;

            TargetInvocationException tie = exception as TargetInvocationException;
            if ((tie != null) && (tie.InnerException != null))
                return GetInterestingException(tie.InnerException);

            return exception;
        }

        /// <summary>
        /// Gets all interesting CodeFluent messages.
        /// </summary>
        /// <param name="errors">The errors. May be null.</param>
        /// <returns>A concatenated list of messages.</returns>
        public static string[] GetAllMessages(IEnumerable<string> errors)
        {
            DistinctDictionary<int> codes = new DistinctDictionary<int>();
            List<string> msgs = new List<string>();
            if (errors != null)
            {
                foreach (string error in errors)
                {
                    AddMessages(error, codes, msgs);
                }
            }
            if ((msgs.Count == 0) && (errors != null))
            {
                msgs.AddRange(errors);
            }
            return msgs.ToArray();
        }

        private static void AddMessages(string error, DistinctDictionary<int> codes, List<string> msgs)
        {
            if (error == null)
                return;

            string current = error;
            do
            {
                // format is "CFXXXX:"
                int pos = current.LastIndexOf("CF");
                if (pos < 0)
                    break;

                if ((pos + 7) > current.Length)
                    break;

                string scode = current.Substring(pos + 2, 4);
                int code;
                if (!int.TryParse(scode, out code))
                    break;

                string err = current.Substring(pos).Trim();
                int eol = err.IndexOf(Environment.NewLine);
                if (eol >= 0)
                {
                    err = err.Substring(0, eol).Trim();
                }
                if ((err.Length > 0) && (!codes.Contains(code)))
                {
                    err = err.Replace(@"\r", "").Replace(@"\n", "");
                    msgs.Insert(0, err);
                    codes.Add(code);
                }
                current = current.Substring(0, pos);
            }
            while (true);
        }

        /// <summary>
        /// Gets all interesting CodeFluent messages.
        /// </summary>
        /// <param name="error">The error. May be null.</param>
        /// <returns>A concatenated list of messages.</returns>
        public static string[] GetAllMessages(string error)
        {
            DistinctDictionary<int> codes = new DistinctDictionary<int>();
            List<string> msgs = new List<string>();
            AddMessages(error, codes, msgs);
            if (msgs.Count == 0)
            {
                msgs.Add(error);
            }
            return msgs.ToArray();
        }

        /// <summary>
        /// Gets the interesting exception message from an exception.
        /// </summary>
        /// <param name="exception">The original exception. May be null.</param>
        /// <returns>An exception message.</returns>
        public static string GetInterestingExceptionMessage(Exception exception)
        {
            if (exception == null)
                return null;

            exception = GetInterestingException(exception);
            if (exception == null)
                return null;

            return exception.Message;
        }

        private static void AppendMessages(StringBuilder sb, Exception e, string separator)
        {
            if (e == null)
                return;

            // this one is not interesting...
            if (!(e is TargetInvocationException))
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                // HACK: since we get all message, we remove this otherwise useful message
                sb.Append(e.Message.Replace("Check inner exception details.", ""));
            }
            AppendMessages(sb, e.InnerException, separator);
        }
    }
}
