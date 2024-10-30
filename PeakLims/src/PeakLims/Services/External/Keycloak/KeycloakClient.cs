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
    Task<UserRepresentation> GetUserByEmail(string email);
}

// rest api https://www.keycloak.org/docs-api/latest/rest-api/index.html 
public class KeycloakClient(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PeakLimsOptions> options) : IKeycloakClient
{
    private readonly PeakLimsOptions.AuthOptions _options = options.Value.Auth;
    private HttpClient GetKeycloakClient() => httpClientFactory.CreateClient(Consts.HttpClients.KeycloakAdmin);
    private Uri UsersRoute() => new Uri(_options.Administration.BaseApiRoute).Append("/admin/realms/PeakLIMS/users");
    private Uri TokenRoute() => new(_options.TokenUrl);
    
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

    // needs service role accounts: https://stackoverflow.com/questions/60359979/keycloak-admin-rest-api-unknown-error-for-update-user-api
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
    
    public async Task<UserRepresentation> GetUserByEmail(string email)
    {
        var token = await GetTokenAsync();
        var client = GetKeycloakClient();
        
        var baseUri = UsersRoute();
        var uriBuilder = new UriBuilder(baseUri);
        var query = $"email={Uri.EscapeDataString(email)}";
        uriBuilder.Query = query;
        var requestUri = uriBuilder.Uri;
        
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserRepresentation>>(content, JsonSerializationOptions.LlmSerializerOptions);

        return users?.FirstOrDefault();
    }

}
