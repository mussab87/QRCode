using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Utility.Errors;

namespace Utility.Errors
{
    [DataContract]
    public class BusinessFault
    {        
        public BusinessFault() { }

        public BusinessFault(string errorMessage, string messageResourceKey)
        {
            MessageResourceKey = messageResourceKey;
            ErrorMessage = errorMessage;
        }

        public BusinessFault(ErrorCodes errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public BusinessFault(GenException rex)
        {
            ErrorCode = rex.StatusCode;
            ErrorMessage = rex.ErrorMessage;
        }

        public BusinessFault(LocalizedException rex)
        {
            ErrorCode = rex.StatusCode;
            ErrorMessage = rex.ErrorMessage;
            MessageResourceKey = rex.MessageResourceKey;
        }

        [DataMember]
        public ErrorCodes ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string MessageResourceKey { get; set; }
    }
}