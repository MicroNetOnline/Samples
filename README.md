# Authentication & Authorization Samples

The following code examples are for customers integrating with GrowthZone. The GrowthZone server is both an OAuth2 and OpenID Connect server so it can be used by customers wishing to integrate with the GrowthZone APIs or using GrowthZone as their Single-Sign-On identity provider. 

OpenID Connect and OAuth2, whilst similar in appearance, serve two different purposes. 

OAuth2 is an authorization framework that enables third party applications to obtain access to user account information via the GrowthZone API. It does this by allowing the third party application to delegate authentication to the GrowthZone server and then have the user provide their authorization for the third party application to access their information. It supports different ways to achive this depending on the needs of the third party application.

OpenID Connect builds on top of OAuth2 and provides federated authentication. It does this by also delegating its authentication to the GrowthZone server but instead of the GrowthZone server providing an access token for use on it's API, it provides a specified token that the third party application can use to identify the user on their own system. The GrowthZone server then provides the necessary endpoints that enable the third party application to validate the token to ensure that it is valid and hasn't been tampered with.

The examples provided cover authentication via the oAuth2 Framework and authorization via OpenID Connect. 

## PHP Examples

[PHP](PHP/)

## C Sharp Examples

[CSharp](CSharp/)
