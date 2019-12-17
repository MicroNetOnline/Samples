using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using GrowthZone.Shared;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using OpenIDConnect;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace OpenIDConnect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app) 
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    AuthenticationType = "Cookies"
                });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,

                    Authority = GrowthZoneClient.Host,
                    MetadataAddress = $"{GrowthZoneClient.Host}/openid/well-known/openid-configuration",
                    RequireHttpsMetadata = false,

                    ClientId = GrowthZoneClient.ClientId,
                    ClientSecret = GrowthZoneClient.ClientSecret,
                    RedirectUri = "http://localhost:44943/openid/callback",
                    PostLogoutRedirectUri = "http://localhost:44943/",
                    RedeemCode = true,
                    SaveTokens = true,

                    // We set the response type to be code + id_token as we want to use GrowthZone as an SSO provider
                    // and also access the GrowthZone API on behalf of the user. If you want to use GrowthZone purely 
                    // as an SSO provider then the ResponseType can just be "id_token". 
                    ResponseType = "code",

                    // The scope below must include "openid" in order for the id_token to be returned.
                    Scope = "openid profile email offline_access",

                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                        ValidateIssuer = false
                    },
                    ProtocolValidator = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectProtocolValidator
                    {
                        RequireNonce = false,
                        RequireState = false,
                        RequireStateValidation = false
                    },

                    SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,

                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        TokenResponseReceived = n =>
                        {
                            // track the access token for later use
                            n.OwinContext.Set("GrowthZone:AccessToken", n.TokenEndpointResponse.AccessToken);
                            n.OwinContext.Set("GrowthZone:IdToken", n.TokenEndpointResponse.IdToken);

                            return Task.CompletedTask;
                        },

                        SecurityTokenValidated = async n =>
                        {
                            var accessToken = n.OwinContext.Get<string>("GrowthZone:AccessToken");
                            var idToken = n.OwinContext.Get<string>("GrowthZone:IdToken");

                            n.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", accessToken));
                            n.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", idToken));

                            // using the Access Token we can access the Userinfo  
                            // endpoint to return more information about the user
                            using (var httpClient = new HttpClient { BaseAddress = new Uri(GrowthZoneClient.Host) })
                            {
                                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                                
                                var response = await httpClient.GetAsync("/oauth/userinfo");
                                
                                if (response.IsSuccessStatusCode == false)
                                {
                                    return;
                                }

                                var properties = await response.Content.ReadAsJsonAsync<IDictionary<string, string>>();

                                n.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.GivenName, properties["given_name"]));
                                n.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Surname, properties["family_name"]));
                            }
                        },

                        RedirectToIdentityProvider = n =>
                        {
                            if (n.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectRequestType.Logout)
                            {
                                var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                                if (idTokenHint != null)
                                {
                                    n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                }
                            }

                            return Task.CompletedTask;
                        }
                    }
                });
        }
    }
}