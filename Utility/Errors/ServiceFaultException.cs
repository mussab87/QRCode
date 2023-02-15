using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Utility.Core.Logging;
using Utility.Errors;

namespace Utility.Errors
{
    [DataContract]
    public class ServiceFaultException : Exception
    {
        [DataMember]
        public ErrorCodes StatusCode;

        [DataMember]
        public string ErrorMessage;

        public ServiceFaultException()
        {
        }

        protected ServiceFaultException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public ServiceFaultException(ErrorCodes statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
            this.ErrorMessage = message;
        }

        public ServiceFaultException(int errorCode, string message) : base(message)
        {
            this.StatusCode = (ErrorCodes)errorCode;
            this.ErrorMessage = message;
        }

        public ServiceFaultException(ErrorCodes statusCode, string message, params string[] args) : base(ErrorMessages.ParseErrorTokens(message, args))
        {
            string m = ErrorMessages.ParseErrorTokens(message, args);

            this.StatusCode = statusCode;
            this.ErrorMessage = m;
        }

        public ServiceFaultException(string message) : base(message)
        {
            this.StatusCode = ErrorCodes.Empty;
        }

        public ServiceFaultException(string message, params string[] args) : base(ErrorMessages.ParseErrorTokens(message, args))
        {
            string m = ErrorMessages.ParseErrorTokens(message, args);

            this.StatusCode = ErrorCodes.Empty;
            this.ErrorMessage = m;
        }

        public ServiceFaultException(string message, Exception exception) : base(message, exception)
        {
            this.StatusCode = ErrorCodes.Empty;
            this.ErrorMessage = message + "{" + exception.Message + "}";
        }

        public ServiceFaultException(Exception exception)
        {
            this.StatusCode = ErrorCodes.Empty;
            this.ErrorMessage = "{" + exception.Message + "}";
        }
    }
}