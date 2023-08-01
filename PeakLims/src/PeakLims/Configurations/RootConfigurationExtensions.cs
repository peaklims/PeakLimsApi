namespace PeakLims.Configurations;

public static class RootConfigurationExtensions
{
    public static string GetJaegerHostValue(this IConfiguration configuration)
        => configuration.GetSection("JaegerHost").Value;
}