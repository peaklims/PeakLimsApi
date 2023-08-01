namespace KeycloakInPulumi;

using KeycloakInPulumi.Extensions;
using KeycloakInPulumi.Factories;
using Pulumi;
using Pulumi.Keycloak;
using Pulumi.Keycloak.Inputs;

class RealmBuild : Stack
{
    public RealmBuild()
    {
        var realm = new Realm("DevRealm-realm", new RealmArgs
        {
            RealmName = "DevRealm",
            RegistrationAllowed = true,
            ResetPasswordAllowed = true,
            RememberMe = true,
            EditUsernameAllowed = true
        });
        var peaklimsScope = ScopeFactory.CreateScope(realm.Id, "peak_lims");
        
        var peakLimsPostmanMachineClient = ClientFactory.CreateClientCredentialsFlowClient(realm.Id,
            "peak_lims.postman.machine", 
            "dd283422-f6ef-4e28-b373-1d3b9d909f8e", 
            "PeakLims Postman Machine",
            "https://oauth.pstmn.io");
        peakLimsPostmanMachineClient.ExtendDefaultScopes(peaklimsScope.Name);
        peakLimsPostmanMachineClient.AddAudienceMapper("peak_lims");
        
        var peakLimsPostmanCodeClient = ClientFactory.CreateCodeFlowClient(realm.Id,
            "peak_lims.postman.code", 
            "946407b0-ce34-40ee-a0d6-205b813588f3", 
            "PeakLims Postman Code",
            "https://oauth.pstmn.io",
            redirectUris: null,
            webOrigins: null
            );
        peakLimsPostmanCodeClient.ExtendDefaultScopes(peaklimsScope.Name);
        peakLimsPostmanCodeClient.AddAudienceMapper("peak_lims");
        
        var peakLimsSwaggerClient = ClientFactory.CreateCodeFlowClient(realm.Id,
            "peak_lims.swagger", 
            "9bbea418-bed9-4f32-8e5f-12a11d389559", 
            "PeakLims Swagger",
            "https://localhost:5227",
            redirectUris: null,
            webOrigins: null
            );
        peakLimsSwaggerClient.ExtendDefaultScopes(peaklimsScope.Name);
        peakLimsSwaggerClient.AddAudienceMapper("peak_lims");
        
        var peakLimsNextClient = ClientFactory.CreateCodeFlowClient(realm.Id,
            "peak_lims.next", 
            "063c2754-b258-47a4-92bc-22a609e7ca13", 
            "PeakLims Next",
            "http://localhost:6488",
            redirectUris: new InputList<string>() 
                {
                "http://localhost:6488/*",
                },
            webOrigins: new InputList<string>() 
                {
                "https://localhost:5227",
                "http://localhost:6488",
                }
            );
        peakLimsNextClient.ExtendDefaultScopes(peaklimsScope.Name);
        peakLimsNextClient.AddAudienceMapper("peak_lims");
        
        var bob = new User("bob", new UserArgs
        {
            RealmId = realm.Id,
            Username = "bob",
            Enabled = true,
            Email = "bob@domain.com",
            FirstName = "Smith",
            LastName = "Bobson",
            InitialPassword = new UserInitialPasswordArgs
            {
                Value = "bob",
                Temporary = true,
            },
        });

        var alice = new User("alice", new UserArgs
        {
            RealmId = realm.Id,
            Username = "alice",
            Enabled = true,
            Email = "alice@domain.com",
            FirstName = "Alice",
            LastName = "Smith",
            InitialPassword = new UserInitialPasswordArgs
            {
                Value = "alice",
                Temporary = true,
            },
        });
    }
}