using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GrowthZone.Shared;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;

namespace OpenIDConnect.Controllers
{
    public class HomeController : Controller
    {
        static HomeController()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // ignore certificate errors when testing
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/home/claims"                
            };

            HttpContext.GetOwinContext().Authentication.Challenge(properties, OpenIdConnectAuthenticationDefaults.AuthenticationType);

            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }

        [HttpGet, Authorize]
        public ActionResult Logout()
        {
            var manager = HttpContext.GetOwinContext().Authentication;

            manager.SignOut(Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType, OpenIdConnectAuthenticationDefaults.AuthenticationType);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> Claims()
        {
            var user = User as ClaimsPrincipal;

            // the access token for the GrowthZone API is contained as a claim
            var accessToken = user.FindFirst("access_token").Value;
            
            using (var client = new GrowthZoneClient(new Uri(GrowthZoneClient.Host), accessToken))
            {
                var claims = await client.GetClaimsAsync();

                return View(claims);
            }
        }
    }
}