using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack.Text;
using Utility.Core.Logging;

namespace Utility.Toolkit.Data
{
    public class SerializeManager
    {
        public static string SerializeRequest<T>(object request)
        {
            FileTrace.WriteMemberEntry();

            string json = null;
            try
            {
                Type stringType = typeof(T);
                if (stringType == typeof(string))
                {
                    // Check if the original request was already a json.
                    var str = Convert.ToString(request);
                    var isAlreadyJson = IsValidJson(str);
                    if (isAlreadyJson)
                        return str;
                }

                json = ServiceStack.Text.JsonSerializer.SerializeToString(request);
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
            }
            finally
            {
                FileTrace.WriteMemberExit();
            }
            return json;
        }

        public static T DeserializeRequest<T>(string request)
        {
            FileTrace.WriteMemberEntry();

            T obj = default(T);
            try
            {
                using (var stream = GenerateStreamFromString(request))
                {
                   obj = ServiceStack.Text.JsonSerializer.DeserializeFromStream<T>(stream);
                }
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
            }
            finally
            {
                FileTrace.WriteMemberExit();
            }
            return obj;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}