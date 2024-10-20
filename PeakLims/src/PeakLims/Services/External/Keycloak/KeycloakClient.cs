namespace PeakLims.Services.External.Keycloak;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain.Users;
using Microsoft.Extensions.Options;
using Models;
using Resources;
using Serilog;
using Utilities;

public interface IKeycloakClient
{
    Task<string> GetTokenAsync();
    Task CreateUserAsync(UserRepresentation user);
}

public class KeycloakClient(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PeakLimsOptions> options) : IKeycloakClient
{
    private readonly PeakLimsOptions.AuthOptions _options = options.Value.Auth;
    private HttpClient GetKeycloakClient() => httpClientFactory.CreateClient(Consts.HttpClients.KeycloakAdmin);
    private Uri UsersRoute() => new Uri(_options.Administration.BaseApiRoute).Append("/auth/admin/realms/PeakLIMS/users");
    private Uri TokenRoute() => new Uri(_options.Administration.BaseApiRoute).Append("/auth/realms/PeakLIMS/protocol/openid-connect/token");
    
    // private string UsersRoute() => "users";
    // private string TokenRoute() => "protocol/openid-connect/token";
    
    public async Task<string> GetTokenAsync()
    {
        var client = GetKeycloakClient();

        var parameters = new Dictionary<string, string>
        {
            { "client_id", _options.Administration.ClientId },
            { "client_secret", _options.Administration.ClientSecret },
            { "grant_type", "client_credentials" }
        };

        var content = new FormUrlEncodedContent(parameters);
        var response = await client.PostAsync(TokenRoute(), content);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(result);
        
        return tokenResponse.AccessToken;
    }

    public async Task CreateUserAsync(UserRepresentation user)
    {
        // TODO do some caching to avoid getting a new token every time
        var token = await GetTokenAsync();
        var client = GetKeycloakClient();

        var userJson = JsonSerializer.Serialize(user, JsonSerializationOptions.LlmSerializerOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, UsersRoute())
        {
            Content = new StringContent(userJson, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
    }
}
