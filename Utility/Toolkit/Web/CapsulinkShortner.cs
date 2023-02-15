using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility.Core.Logging;

namespace Utility.Toolkit.Web
{
    public class CapsulinkShortner
    {
        public static string Shorten(string urlToShorten)
        {
            string capsulinkUrl = "https://www.capsulink.com";
            string apiKey = "fb2f5898d43dd95db484d697f31c8f6a19f2bdd3";

            return Shorten(capsulinkUrl, apiKey, urlToShorten, false);
        }

        public static string Shorten(string capsulinkUrl, string apiKey, string urlToShorten, bool throwError)
        {
            FileTrace.WriteMemberEntry();
            CapsulinkShortnerModel capsulinkShortnerModel = null;

            try
            {
                var parameters = ParseToURL(urlToShorten);

                var commandBuilder = new StringBuilder();
                commandBuilder.AppendLine("{");
                commandBuilder.AppendLine("    \"url\": " + parameters);
                commandBuilder.AppendLine("}");

                var command = commandBuilder.ToString();

                var client = new RestClient(capsulinkUrl);
                var request = new RestRequest("api/capsulate", Method.POST);
                request.AddHeader("Api-Key", apiKey);
                request.AddParameter("application/json", command, ParameterType.RequestBody);

                var response = client.Execute(request);

                // Evaluate the response.
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Unable to capsulinkShortner - Failed HTTP request");

                var content = response.Content;
                if (string.IsNullOrWhiteSpace(content))
                    throw new Exception("Unable to capsulinkShortner from HTTP OK request");

                var exProReturn = JsonConvert.DeserializeObject<CapsulinkShortnerModel>(content);
                if (exProReturn == null || exProReturn.status == null)
                    throw new Exception("Unable to capsulinkShortner from successful request");

                capsulinkShortnerModel = exProReturn;
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                if (throwError)
                    throw;
                else
                    return urlToShorten;
            }
            finally
            {
                FileTrace.WriteMemberExit();
            }
            return capsulinkShortnerModel.data.redirect_url;
        }

        public static string ParseToURL(string requestURL)
        {
            FileTrace.WriteMemberEntry();

            string parsed = null;
            try
            {
                // Serialize back into json.
                string json = JsonConvert.SerializeObject(requestURL);

                parsed = json.ToString();
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                throw;
            }
            FileTrace.WriteMemberExit();
            return parsed;
        }

    }

    public class CapsulinkShortnerModel
    {
        public string status { get; set; }
        public CapsulinkShortnerData data { get; set; }
    }

    public class CapsulinkShortnerData
    {
        public string id { get; set; }
        public string short_url { get; set; }
        public string redirect_url { get; set; }
        public string stats_url { get; set; }
        public string url { get; set; }
        public string folder { get; set; }
    }
}
