namespace KeycloakInPulumi.Extensions;

using Pulumi;
using Pulumi.Keycloak.OpenId;

public static class ClientExtensions
{
    public static void ExtendDefaultScopes(this Client client, params Output<string>[] scopeNames)
    {
        var defaultScopes = client.Name.Apply(clientName =>
            new ClientDefaultScopes($"default-scopes-for-{clientName}", new ClientDefaultScopesArgs()
            {
                RealmId = client.RealmId,
                ClientId = client.Id,
                DefaultScopes =
                {
                    "openid",
                    "profile",
                    "email",
                    "roles",
                    "web-origins",
                    scopeNames,
                },
            })
        );
    }
    
    public static void AddAudienceMapper(this Client client, string audience)
    {
        var audienceMapper = client.Name.Apply(clientName =>
            new AudienceProtocolMapper($"audienceMapper-{clientName}-{audience}", new AudienceProtocolMapperArgs
            {
                RealmId = client.RealmId,
                ClientId = client.Id,
                IncludedCustomAudience = audience,
                Name = $"{audience}-Mapping"
            })
        );
    }
}