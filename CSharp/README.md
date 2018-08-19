# CSharp Samples

## Getting Started
All of the samples will require an OAuth Client ID & Client Secret. You can create your Client ID & Client Secret from the admin area from within your GrowthZone account.

Once you have created your credentials you will need to edit the GrowthZoneClient.cs file and modify the settings to include your newly created credentials. 

    public const string Host = "https://growthzone.growthzonedev.com";
    public const string ClientId = "<client id>";
    public const string ClientSecret = "<client secret>";

The samples rely on the *Thinktecture.IdentityModel.Client* package from NuGet to handle the intricacies of dealing with an OAuth server.

The package can be found here;
https://www.nuget.org/packages/Thinktecture.IdentityModel.Client/

## PasswordGrant
The PasswordGrant sample demonstrates the use of the *password* grant type. This is the simpliest of the OAuth flows and consists of the application providing form to capture the username and password of the user and exchanging those credentials for an access token.

    POST /oauth/token HTTP/1.1
    Host: growthzonedev.com
    Content-type: application/x-www-form-urlencoded

    grant_type=password&username=<username>&<password>=1234luggage&client_id=<clientid>&client_secret=<client_secret>

## AuthorizationCodeFlow
The AuthorizationCodeFlow sample demonstrates the use of the *code* flow. This is the preferrable method of obtaining an access token but is a little more complicated than of the *password* grant type. 

The *code* flow starts by the application redirecting the user to the authorization page on the GrowthZone server. The user will enter their credentials into the login page on the GrowthZone server and upon granting access to the client application the GrowthZone server will redirect back to the client application with an authorization code. This authorization code is a one-time-only token that can be used to exchange for an access token.

## OpenIDConnect
The OpenIDConnect sample demonstrates the use of a Single Sign On approach using OpenID connect. This sample utilizes the *Microsoft.Owin.Security.OpenIdConnect* package from Microsoft. 

https://www.nuget.org/packages/Microsoft.Owin.Security.OpenIdConnect