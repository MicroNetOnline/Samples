using System;
using System.Net.Http;
using System.Net.Http.Headers;
using GrowthZone.Shared;

namespace ApiKeyAuthentication
{
    class Program
    {
        //const string Host = "http://micronet.localtest.me:12221";
        const string Host = "https://cain.growthzoneapp.com/";
        const string ApiKey = "3UHLja5EHFC6SM54oa3u21o4PRRWawTymRjeqKpl";
        
        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Host) })
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", ApiKey);

                var httpResponse = httpClient.GetAsync("api/contacts/root/claims").Result;
                httpResponse.EnsureSuccessStatusCode();

                Console.WriteLine("Status Code={0}", httpResponse.StatusCode);

                var claimsResponse = httpResponse.Content.ReadAsJsonAsync<ClaimsResponse>().Result;
                foreach (var claim in claimsResponse.Claims)
                {
                    Console.WriteLine("{0} = {1}", claim.Name, claim.Value);
                }
            }
        }
    }
}