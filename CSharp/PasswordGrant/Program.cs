using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using GrowthZone.Shared;
using Thinktecture.IdentityModel.Client;

namespace PasswordGrant
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = IgnoreCertificateValidationFailureForTestingOnly;

            Console.WriteLine("Please enter your username:");
            var username = Console.ReadLine();

            Console.WriteLine("\nPlease enter your password:");
            var password = Console.ReadLine();

            var oAuthClient = new OAuth2Client(
                new Uri($"{GrowthZoneClient.Host}/oauth/token"),
                GrowthZoneClient.ClientId, 
                GrowthZoneClient.ClientSecret, 
                OAuth2Client.ClientAuthenticationStyle.PostValues);

            var response = oAuthClient.RequestResourceOwnerPasswordAsync(username, password).Result;
            Console.WriteLine(response.AccessToken);

            Console.WriteLine("\n\nDisplaying your claims.");
            using (var client = new GrowthZoneClient(new Uri(GrowthZoneClient.Host), response.AccessToken))
            {
                var claims = client.GetClaimsAsync().Result;

                foreach (var claim in claims.Claims)
                {
                    Console.WriteLine("{0}: {1}", claim.Name, claim.Value);
                }
            }
        }

        static bool IgnoreCertificateValidationFailureForTestingOnly(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
