namespace Teammato.Services;

public class FailedAccessTokenRequestException : Exception
{
    public FailedAccessTokenRequestException():base("Failed access token request") { }
}