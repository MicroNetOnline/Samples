using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GrowthZone.Shared;
using Thinktecture.IdentityModel.Client;

namespace AuthorizationCodeFlow.Controllers
{
    public class HomeController : Controller
    {
        const string RedirectUri = "http://localhost:62235/home/oauthcallback";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            var client = new OAuth2Client(new Uri($"{GrowthZoneClient.Host}/oauth/authorize"));
            var authorizeUrl = client.CreateAuthorizeUrl(GrowthZoneClient.ClientId, "code", redirectUri: RedirectUri);

            return Redirect(authorizeUrl);
        }

        public async Task<ActionResult> OAuthCallback(string code, string error)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // ignore certificate errors when testing
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            
            if (error != null)
            {
                // TODO: the authorization failed
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }

            var authClient = new OAuth2Client(
                new Uri($"{GrowthZoneClient.Host}/oauth/token"),
                GrowthZoneClient.ClientId, 
                GrowthZoneClient.ClientSecret, 
                OAuth2Client.ClientAuthenticationStyle.PostValues);

            var tokenResponse = await authClient.RequestAuthorizationCodeAsync(code, RedirectUri);

            if (tokenResponse.IsError)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            using (var client = new GrowthZoneClient(new Uri(GrowthZoneClient.Host), tokenResponse.AccessToken))
            {
                var claims = await client.GetClaimsAsync();

                return View("Claims", claims);
            }
        }
    }
}