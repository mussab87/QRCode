using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Utility.Errors;

namespace Utility.Errors
{
    [DataContract]
    public class LocalizedException : GenException
    {        
        [DataMember]
        public string MessageResourceKey { get; set; }

        public LocalizedException(ErrorCodes statusCode, string message, string messageResourceKey)
            : base(statusCode, message)
        {
            MessageResourceKey = messageResourceKey;
        }

        public LocalizedException(string message, string messageResourceKey)
            : base(message)
        {
            MessageResourceKey = messageResourceKey;
        }

        public LocalizedException(BusinessFault fault)
        {
            MessageResourceKey = fault.MessageResourceKey;
            ErrorMessage = fault.ErrorMessage;
            StatusCode = fault.ErrorCode;
        }
    }
}
