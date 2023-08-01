namespace KeycloakInPulumi.Factories;

using Pulumi;
using Pulumi.Keycloak.OpenId;

public class ScopeFactory
{
    public static ClientScope CreateScope(Output<string> realmId, string scopeName)
    {
        return new ClientScope($"{scopeName}-scope", new ClientScopeArgs()
        {
            Name = scopeName,
            RealmId = realmId,
        });
    }
}