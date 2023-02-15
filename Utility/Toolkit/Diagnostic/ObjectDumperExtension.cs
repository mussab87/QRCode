using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Toolkit.Diagnostic
{
    public static class ObjectDumperExtension
    {
        public static string Dump(this object source)
        {
            return ObjectDumper.Dump(source);
        }

        public static string Dump(this object source, int depth)
        {
            return ObjectDumper.Dump(source, depth);
        }
    }
}
