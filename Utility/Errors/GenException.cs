using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Utility.Errors
{
    [DataContract]
    public class GenException : Exception
    {
        #region Fields

        [DataMember]
        public ErrorCodes StatusCode;

        [DataMember]
        public string ErrorMessage;

        #endregion

        #region Ctor

        public GenException()
        {
        }

        protected GenException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public GenException(ErrorCodes statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
            this.ErrorMessage = message;
        }

        public GenException(int errorCode, string message) : base(message)
        {
            this.StatusCode = (ErrorCodes)errorCode;
            this.ErrorMessage = message;
        }

        public GenException(ErrorCodes statusCode, string message, params string[] args) : base(ErrorMessages.ParseErrorTokens(message, args))
        {
            string m = ErrorMessages.ParseErrorTokens(message, args);

            this.StatusCode = statusCode;
            this.ErrorMessage = m;
        }

        public GenException(string message) : base(message)
        {
            this.StatusCode = ErrorCodes.Empty;
        }

        public GenException(string message, params string[] args) : base(ErrorMessages.ParseErrorTokens(message, args))
        {
            string m = ErrorMessages.ParseErrorTokens(message, args);

            this.StatusCode = ErrorCodes.Empty;
            this.ErrorMessage = m;
        }

        public GenException(string message, Exception exception) : base(message, exception)
        {
            this.StatusCode = ErrorCodes.Empty;
            this.ErrorMessage = message + "{" + exception.Message + "}";
        }

        #endregion
    }
}
