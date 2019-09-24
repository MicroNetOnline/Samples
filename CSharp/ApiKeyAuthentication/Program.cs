using System;
using System.Net.Http;
using GrowthZone.Shared;

namespace ApiKeyAuthentication
{
    class Program
    {
        const string Host = "http://test1.localtest.me:12221";
        const string ApiKey = "uPksiV351SYVqhmotkiATUTCr1E3BWBNYAUG4fA";
        
        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Host) })
            {
                httpClient.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

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