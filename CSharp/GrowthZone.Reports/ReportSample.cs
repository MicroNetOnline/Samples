using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GrowthZone.Reports
{
    public class ReportSample
    {
        RestClient Client { get; }
        AuthenticationHeaderValue AuthToken { get; }

        string BaseReportPath = "/api/ReportExecution"; 

        public ReportSample(string host, string username, string password, string clientId, string clientSecret)
        {
            Client = new RestClient(host);

            // Log in
            string oAuthEndpoint = $"{host}/oauth/token";

            var request = new RestRequest("/oauth/token", Method.POST);
            request.AddParameter(new Parameter(
                @"application/x-www-form-urlencoded", 
                $"grant_type=password&username={username}&password={password}&client_id={clientId}&client_secret={clientSecret}",
                @"application/x-www-form-urlencoded", 
                ParameterType.RequestBody));

            var tokenResponse = Client.Execute(request);
            string response = tokenResponse.Content;

            if (!tokenResponse.IsSuccessful)
                throw new InvalidOperationException($"Unable to login with {username}:{password}. \n{response}");

            var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            AuthToken = new AuthenticationHeaderValue("bearer", responseObject["access_token"]);

        }

        private T MakeRequest<T>(string path, Method method, object body = null)
        {
            var request = new RestRequest()
            {
                Method = method,
                Resource = path,
            };

            request.AddHeader("Authorization", $"{AuthToken.Scheme} {AuthToken.Parameter}");

            if (body != null)
                request.AddParameter(@"application/json", body, ParameterType.RequestBody);

            var response = Client.Execute(request);

            if (typeof(T) == typeof(string))
                return (T)(object)response.Content;

            return JsonConvert.DeserializeObject<T>(response.Content, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        // Executes the specified report in Json Format
        public string ExecuteReport(string reportName, int reportId)
        {
            // Get the 'Base' report
            var baseReport = MakeRequest<JObject>($"{BaseReportPath}/{reportName}/{reportId}", Method.GET);

            // Configure changes on baseReport. If we reference the MMP assemblies, then we can deserialize into the ReportObject but for now, just use JObjects.
            // Major configuration options:

            // Add a display field:
            // In the Fields property, select the fields to display by setting the IsSelected Value to True on the desired field.

            // Add A criteria item
            // In the FilterItems property, add a new entry with the applicable details

            // Execute the report
            // The query part is what determines the report type: format=json
            var reportJson = MakeRequest<string>($"{BaseReportPath}/{reportName}?format=json", Method.POST, baseReport);
            return reportJson;
        }


    }
}
