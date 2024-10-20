namespace PeakLims.Services.External.Keycloak.Models;

using System.Text.Json.Serialization;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}