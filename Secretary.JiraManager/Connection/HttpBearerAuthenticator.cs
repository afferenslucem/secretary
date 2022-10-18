using RestSharp;
using RestSharp.Authenticators;

namespace Secretary.JiraManager.Connection;

public class HttpBearerAuthenticator : AuthenticatorBase
{
    public HttpBearerAuthenticator(string PAT)
        : base(PAT) { }
    
    [Obsolete("Obsolete")]
    protected override Parameter GetAuthenticationParameter(string accessToken)
        => new Parameter("Authorization", $"Bearer {accessToken}", ParameterType.HttpHeader);
}