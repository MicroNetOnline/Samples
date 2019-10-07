# Provider Initiated Single Sign On with OpenID Connect
The most common use case for Single Sign On (SSO) is when the SSO request originates from the Service Provider (SP). This would usually occur with a user wanting to perform some authenticated function on the SP and the SP then redirecting the user to the Identity Provider (IdP) for sign-on. 

The other use case for SSO is when the user is operating within the IdP and then wishes to navigate to the SP using their current credentials to avoid the manual login procedure. This is referred to as a *Provider Initiated Single Sign On*.

OpenID Connect (ODIC) doesn't have any native support for *Provider Initiated Single Sign On*, however it can stil be implemented on the SP with realtive ease. 

## How it works
In order for this to work, the IdP needs some way of notifying the SP that an OIDC workflow is required. This can be achieved easily by the SP implementing an endpoint that the SP can call as a Hyperlink, for example; 

    https://example.com/sso/growthzone?url=/forums/community

In this case the SP can identity that the SSO originated from GrowthZone and the users desired destination within the SP is the */forums/community* endpoint.

Once this endpoint has been reached on the SP, the SP can then initiate an OIDC *Authorization Code Flow* workflow. Given that the user is already logged into GrowthZone they will **not** be asked to re-enter their credentials and the *Identity Token* will be passed straight back to the SP. The SP can then perform the authentication process as they would normally for a SP initiated SSO and redirect the user to their desired endpoint destination. 

Whilst this approach will perform several redirections between the IdP and the SP, the user will be oblivious to this occurring and the SSO will leverage the security that is provided with a standard OIDC *Authorization Code Flow* workflow.

## Authorization Grants
The first time a user is providing access to a SP through one of the OpenID Connect flows, the user will be presented with an Authorization Grant screen which informs them of the application that they will be granting access to along with the permissions that they will be granting. Once they approve this grant the first time it will be remebered upon subsequent login attempts and thus the *Provider Initiated Single Sign On* solution will involve no user interaction.