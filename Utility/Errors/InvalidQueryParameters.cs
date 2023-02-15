using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Utility.Errors;

namespace Utility.Core.Errors
{
    [DataContract]
    public class InvalidQueryParameter : GenException
    {
        [DataMember]
        public string QueryParameter;

        public InvalidQueryParameter(ErrorCodes errorCode, string queryParameter, string message, params string[] args)
            : base(errorCode, message, args)
        {
            QueryParameter = queryParameter;
        }
    }
}
