namespace PeakLims.Configurations;

using Microsoft.Extensions.Options;

public class PeakLimsOptions
{
    public const string SectionName = "PeakLims";
    
    public RabbitMqOptions RabbitMq { get; set; } = new RabbitMqOptions();
    public ConnectionStringOptions ConnectionStrings { get; set; } = new ConnectionStringOptions();
    public AuthOptions Auth { get; set; } = new AuthOptions();
    public string JaegerHost { get; set; } = String.Empty;
    public string LocalstackPort { get; set; } = String.Empty;
    
    public class RabbitMqOptions
    {
        public const string SectionName = $"{PeakLimsOptions.SectionName}:RabbitMq";
        public const string HostKey = nameof(Host);
        public const string VirtualHostKey = nameof(VirtualHost);
        public const string UsernameKey = nameof(Username);
        public const string PasswordKey = nameof(Password);
        public const string PortKey = nameof(Port);

        public string Host { get; set; } = String.Empty; // "localhost";
        public string VirtualHost { get; set; } = String.Empty; // "/";
        public string Username { get; set; } = String.Empty; // "guest";
        public string Password { get; set; } = String.Empty; // "guest";
        public string Port { get; set; } = String.Empty; // "57481";
    }

    public class ConnectionStringOptions
    {
        public const string SectionName = $"{PeakLimsOptions.SectionName}:ConnectionStrings";
        public const string PeakLimsKey = nameof(PeakLims); 
            
        public string PeakLims { get; set; } = String.Empty;
    }
    
    
    public class AuthOptions
    {
        public const string SectionName = $"{PeakLimsOptions.SectionName}:Auth";

        public string Audience { get; set; } = String.Empty;
        public string Authority { get; set; } = String.Empty;
        public string AuthorizationUrl { get; set; } = String.Empty;
        public string TokenUrl { get; set; } = String.Empty;
        public string ClientId { get; set; } = String.Empty;
        public string ClientSecret { get; set; } = String.Empty;
    }
}

public static class PeakLimsOptionsExtensions
{
    public static PeakLimsOptions GetPeakLimsOptions(this IConfiguration configuration)
    {
        return configuration
            .GetSection(PeakLimsOptions.SectionName)
            .Get<PeakLimsOptions>();
    }
    
    public static PeakLimsOptions.RabbitMqOptions GetRabbitMqOptions(this IConfiguration configuration)
    {
        return configuration
            .GetSection(PeakLimsOptions.RabbitMqOptions.SectionName)
            .Get<PeakLimsOptions.RabbitMqOptions>();
    }
    
    public static PeakLimsOptions.ConnectionStringOptions GetConnectionStringOptions(this IConfiguration configuration)
    {
        return configuration
            .GetSection(PeakLimsOptions.ConnectionStringOptions.SectionName)
            .Get<PeakLimsOptions.ConnectionStringOptions>();
    }
    
    public static PeakLimsOptions.AuthOptions GetAuthOptions(this IConfiguration configuration)
    {
        return configuration
            .GetSection(PeakLimsOptions.AuthOptions.SectionName)
            .Get<PeakLimsOptions.AuthOptions>();
    }
    
    public static string GetJaegerHost(this IConfiguration configuration)
    {
        return configuration.GetSection(nameof(PeakLimsOptions.JaegerHost)).Value;
    }
    
    public static string GetLocalstackPort(this IConfiguration configuration)
    {
        return configuration.GetSection(nameof(PeakLimsOptions.LocalstackPort)).Value;
    }
}
