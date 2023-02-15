using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Utility.Core.Utitlites
{
    public static class MethodInfoExtension
    {
        public static string GetMethodSignature(MethodBase method)
        {
            if (method == null) return string.Empty;

            StringBuilder output = new StringBuilder();

            output.Append(method.DeclaringType.Namespace);
            output.Append(".");
            output.Append(method.DeclaringType.Name);
            output.Append(".");
            output.Append(method.Name);
            output.Append("(");

            ParameterInfo[] paramInfos = method.GetParameters();

            if (paramInfos.Length > 0)
            {
                output.Append("{0} {1}".FormatWith(paramInfos[0].ParameterType.Name, paramInfos[0].Name));

                if (paramInfos.Length > 1)
                {
                    for (int j = 1; j < paramInfos.Length; j++)
                    {
                        output.Append(", {0} {1}".FormatWith(paramInfos[j].ParameterType.Name, paramInfos[j].Name));
                    }
                }
            }

            output.Append(")");

            return output.ToString();
        }
    }
}
