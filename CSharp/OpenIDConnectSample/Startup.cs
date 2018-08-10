using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using GrowthZone.Shared;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using OpenIDConnect;
using Owin;
using Thinktecture.IdentityModel.Client;

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

                    ClientId = GrowthZoneClient.ClientId,
                    ClientSecret = GrowthZoneClient.ClientSecret,
                    RedirectUri = "http://localhost:44943/openid/callback",
                    PostLogoutRedirectUri = "http://localhost:44943/",
                    
                    // We set the response type to be code + id_token as we want to use GrowthZone as an SSO provider
                    // and also access the GrowthZone API on behalf of the user. If you want to use GrowthZone purely 
                    // as an SSO provider then the ResponseType can just be "id_token". 
                    ResponseType = "code id_token",

                    // The scope below must include "openid" in order for the id_token to be returned.
                    Scope = "openid profile offline_access",

                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                        //ValidIssuer = "http://micronet.localtest.me:12221/"
                        ValidIssuer = "https://growthzone.gz-10958.growthzonebranch.com/"
                    },

                    SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,

                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        // If the ResponseType above includes "code" then we handle the authorization code 
                        // being returned so we can use it to fetch an access_token from the server
                        AuthorizationCodeReceived = async n =>
                        {
                            var authClient = new OAuth2Client(
                                new Uri($"{GrowthZoneClient.Host}/oauth/token"),
                                GrowthZoneClient.ClientId, 
                                GrowthZoneClient.ClientSecret, 
                                OAuth2Client.ClientAuthenticationStyle.PostValues);
                            
                            var tokenResponse = await authClient.RequestAuthorizationCodeAsync(n.Code, n.RedirectUri);

                            if (tokenResponse.IsError)
                            {
                                throw new Exception(tokenResponse.Error);
                            }

                            // Create a new identity here as opposed to using the identity that was created from the id_token. 
                            // We do this so we can track additional claims such as the access_token for the GrowthZone API.
                            var identity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);

                            identity.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                            identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                            var userClient = new UserInfoClient(new Uri($"{GrowthZoneClient.Host}/oauth/userinfo"), tokenResponse.AccessToken);

                            //// The following is optional if you want additioanl profile information about the user
                            //var userResponse = await userClient.GetAsync();
                            //if (userResponse != null && userResponse.IsError == false)
                            //{
                            //    identity.AddClaims(userResponse.Claims.Select(c => new Claim(c.Item1, c.Item2)));
                            //}

                            n.AuthenticationTicket = new AuthenticationTicket(
                                new ClaimsIdentity(identity.Claims, n.AuthenticationTicket.Identity.AuthenticationType, "name", "role"),
                                n.AuthenticationTicket.Properties);
                        },

                        RedirectToIdentityProvider = n =>
                        {
                            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                            {
                                var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                                if (idTokenHint != null)
                                {
                                    n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                }
                            }

                            return Task.FromResult(0);
                        }
                    } 
                });
        }
    }
}