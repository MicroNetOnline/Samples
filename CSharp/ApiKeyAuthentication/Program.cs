using System;
using System.Net.Http;
using MicroNet.MMP.Shared;

namespace ApiKeyAuthentication
{
    class Program
    {
        const string Host = "http://micronet.localtest.me:12221";
        const string ApiKey = "vwxyz";
        
        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Host) })
            {
                httpClient.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

                var httpResponse = httpClient.GetAsync("api/contacts/root/claims").Result;
                httpResponse.EnsureSuccessStatusCode();

                var claimsResponse = httpResponse.Content.ReadAsJsonAsync<ClaimsResponse>().Result;
                foreach (var claim in claimsResponse.Claims)
                {
                    Console.WriteLine("{0} = {1}", claim.Name, claim.Value);
                }
            }
        }
    }
}