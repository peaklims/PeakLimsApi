namespace PeakLims.Configurations;

public class AuthOptions
{
    public const string SectionName = "Auth";

    public string Audience { get; set; } = String.Empty;
    public string Authority { get; set; } = String.Empty;
    public string AuthorizationUrl { get; set; } = String.Empty;
    public string TokenUrl { get; set; } = String.Empty;
    public string ClientId { get; set; } = String.Empty;
    public string ClientSecret { get; set; } = String.Empty;
}

public static class AuthOptionsExtensions
{
    public static AuthOptions GetAuthOptions(this IConfiguration configuration)
        => configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>();
}