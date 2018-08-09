using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MicroNet.MMP.Shared;
using System.Security.Claims;
using System.Net;
using Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Net.Http;

namespace OpenIDConnectSample.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            var manager = HttpContext.GetOwinContext().Authentication;

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
            
            using (var client = new MemberZoneClient(new Uri(MemberZoneClient.Host), accessToken))
            {
                var claims = await client.GetClaimsAsync();

                return View(claims);
            }
        }
    }
}