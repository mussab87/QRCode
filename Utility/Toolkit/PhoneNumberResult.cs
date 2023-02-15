using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Toolkit
{
    public class PhoneNumberResult
    {
        public bool IsValid { get; set; }
        public string InvalidReasonDescription { get; set; }

        public string ShortNumber { get; set; }
    }
}
