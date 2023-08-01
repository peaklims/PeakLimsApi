namespace PeakLims.FunctionalTests.TestUtilities;

using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Extensions.Services;

public static class HttpClientExtensions
{
    public static HttpClient AddAuth(this HttpClient client, string nameIdentifier = null)
    {
        nameIdentifier ??= Guid.NewGuid().ToString();
        var claims = new Dictionary<string, string>()
        {
            { ClaimTypes.NameIdentifier , nameIdentifier },
        };
        
        client.SetFakeBearerToken(claims);

        return client;
    }

    public static async Task<HttpResponseMessage> GetRequestAsync(this HttpClient client, string url)
    {
        return await client.GetAsync(url).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> DeleteRequestAsync(this HttpClient client, string url)
    {
        return await client.DeleteAsync(url).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> PostJsonRequestAsync(this HttpClient client, string url, object value)
    {
        var options = new JsonSerializerOptions();
        return await client.PostAsJsonAsync(url, value, options).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> PutJsonRequestAsync(this HttpClient client, string url, object value)
    {
        var options = new JsonSerializerOptions();
        return await client.PutAsJsonAsync(url, value, options).ConfigureAwait(false);
    }
}