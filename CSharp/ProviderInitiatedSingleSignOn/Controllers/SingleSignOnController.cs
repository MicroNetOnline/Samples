using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using GrowthZone.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProviderInitiatedSingleSignOn.Models;
using ProviderInitiatedSingleSignOn.Services;

namespace ProviderInitiatedSingleSignOn.Controllers
{
    [Route("sso")]
    public sealed class SingleSignOnController : Controller
    {
        [HttpGet]
        [Route("signout")]
        public async Task<IActionResult> Signout([FromHeader(Name = "Referer")] Uri referer)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(referer.PathAndQuery);
        }

        [HttpGet]
        [Route("growthzone")]
        public IActionResult GrowthZone([FromServices] StateService stateService, string url)
        {
            // this example uses a direct route called "/sso/growthzone" to indicate that the request has
            // come from GrowthZone, however this could also be done using a generic link and looking
            // at the Referer or Origin header to determine where the request came from

            if (User.Identity.IsAuthenticated)
            {
                return Redirect(url);
            }

            // the state serves two purposes
            //   a) it allows an opaque value to be sent with the request and returned with the response 
            //      such that we can ensure that the response originated from our request and also 
            //      ensure that the replay attacks cant occurr where the same state value is used multiple times
            //   b) it allows us to keep track of information accross requests, for example in this case
            //      we want to know the original URL that the user had requested
            var state = stateService.Create(
                new Dictionary<string, string>
                {
                    { "url", url }
                });

            // when the request comes in we redirect to the GrowthZone authorization endpoint
            var parameters = new Dictionary<string, string>
            {
                { "response_type", "code" },
                { "response_mode", "form_post" },
                { "scope", "openid profile offline_access" },
                { "client_id", GrowthZoneClient.ClientId },
                { "redirect_uri", "http://localhost:51843/sso/callback" },
                { "state", state }
            };

            var uri = new UriBuilder(GrowthZoneClient.Host)
            {
                Path = "/oauth/authorize",
                Query = string.Join("&", parameters.Select(k => $"{k.Key}={HttpUtility.UrlEncode(k.Value)}"))
            };

            return Redirect(uri.ToString());
        }

        [HttpPost]
        [Route("callback")]
        public async Task<IActionResult> Callback(
            [FromServices] UserRepository userRepository,
            [FromServices] StateService stateService,
            [FromForm] IFormCollection formParameters)
        {
            // validate the state parameter was created by us 
            // and also that it hasnt previous been used before
            if (stateService.TryValidate(formParameters["state"], out var state) == false)
            {
                return BadRequest();
            }

            // when the user successfully logs into GrowthZone this endpoint 
            // will be called with an authorization code that can be used to
            // exchange for an access token
            using (var httpClient = new HttpClient { BaseAddress = new Uri(GrowthZoneClient.Host) })
            {
                var parameters = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", formParameters["code"] },
                    { "client_id", GrowthZoneClient.ClientId },
                    { "client_secret", GrowthZoneClient.ClientSecret },
                    { "redirect_uri", "http://localhost:51843/sso/callback" }
                };

                var response = await httpClient.PostAsync("/oauth/token", new FormUrlEncodedContent(parameters));
                response.EnsureSuccessStatusCode();

                // the response will contain a few different parts but we are
                // only interested in two, the id_token and the access_token
                var json = await response.Content.ReadAsJsonAsync<IDictionary<string, string>>();

                // decode the ID token so that we can read the claims, but
                // this will only work if the token passes validation first
                var token = await DecodeJwtTokenAsync(httpClient, json["id_token"]);

                // the token will contain information that identifies the user within
                // GrowthZone by using the token.Issuer and token.Subject fields 
                // and also the email address which can identifier the user globally
                var email = token.Claims.Single(claim => claim.Type == ClaimTypes.Email);

                var user = userRepository.Find(email.Value);
                if (user == null)
                {
                    // fetch additional information about the user, this is only available 
                    // for authenticated requests which need the Bearer token set
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", json["access_token"]);
                    response = await httpClient.GetAsync("/oauth/userinfo");

                    var properties = await response.Content.ReadAsJsonAsync<IDictionary<string, string>>();

                    user = new User
                    {
                        Email = email.Value,
                        FirstName = properties["given_name"],
                        LastName = properties["family_name"]
                    };

                    userRepository.Save(user);
                }

                // once we have the user we can sign them in
                var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            }

            return Redirect(state["url"]);
        }

        static async Task<JwtSecurityToken> DecodeJwtTokenAsync(HttpClient httpClient, string token)
        {
            // the first step is to validate the id_token before we can trust it
            // but validation requires the public signing key which can be downloaded
            // from a well-known location on the GrowthZone server
            var signingKeys = JsonWebKeySet.Create(await httpClient.GetStringAsync("openid/well-known/jwks")).GetSigningKeys();

            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = GrowthZoneClient.ClientId,
                ValidateIssuer = true,
                ValidIssuer = GrowthZoneClient.Host + "/",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKeys[0]
            };

            // if the validation has passes without throwing an exception then we can 
            // trust that the token is from GrowthZone and hasnt been tampered with
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var securityToken);

            return securityToken as JwtSecurityToken;
        }
    }
}