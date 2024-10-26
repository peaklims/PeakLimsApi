namespace KeycloakInPulumi;

using KeycloakInPulumi.Extensions;
using KeycloakInPulumi.Factories;
using Pulumi;
using Pulumi.Keycloak.Inputs;
using Keycloak = Pulumi.Keycloak;

class RealmBuild : Stack
{
    public RealmBuild()
    {
        var realm = new Keycloak.Realm("PeakLIMS-realm", new Keycloak.RealmArgs
        {
            RealmName = "PeakLIMS",
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
        peakLimsPostmanMachineClient.ExtendDefaultScopes("peak_lims");
        peakLimsPostmanMachineClient.AddAudienceMapper("peak_lims");
        peakLimsPostmanMachineClient.AddTenantMapper();
        
        var peakLimsPostmanCodeClient = ClientFactory.CreateCodeFlowClient(realm.Id,
            "peak_lims.postman.code", 
            "946407b0-ce34-40ee-a0d6-205b813588f3", 
            "PeakLims Postman Code",
            "https://oauth.pstmn.io",
            redirectUris: null,
            webOrigins: null
            );
        peakLimsPostmanCodeClient.ExtendDefaultScopes("peak_lims");
        peakLimsPostmanCodeClient.AddAudienceMapper("peak_lims");
        peakLimsPostmanCodeClient.AddTenantMapper();
        
        
        var peakLimsSwaggerClient = ClientFactory.CreateCodeFlowClient(realm.Id,
            "peak_lims.swagger", 
            "9bbea418-bed9-4f32-8e5f-12a11d389559", 
            "PeakLims Swagger",
            "https://localhost:5227",
            redirectUris: null,
            webOrigins: null
            );
        peakLimsSwaggerClient.ExtendDefaultScopes("peak_lims");
        peakLimsSwaggerClient.AddAudienceMapper("peak_lims");
        peakLimsSwaggerClient.AddTenantMapper();
        
        var peakLimsSpaClient = ClientFactory.CreateCodeFlowClient(realm.Id,
            "peak_lims.spa", 
            "f8d59983-2a9b-45f4-86ca-c081283c92df", 
            "PeakLims Spa",
            "https://localhost:4378",
            redirectUris: new InputList<string>() 
            {
                "https://localhost:4378/*",
            },
            webOrigins: new InputList<string>() 
            {
                "https://localhost:5227",
                "https://localhost:4378",
            }
        );
        peakLimsSpaClient.ExtendDefaultScopes("peak_lims");
        peakLimsSpaClient.AddAudienceMapper("peak_lims");
        peakLimsSpaClient.AddTenantMapper();
        
        var bob = new Keycloak.User("bob", new Keycloak.UserArgs
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
            Attributes =
            {
                { "organization-id", "84c294a2-ac18-418f-b4e0-d86ce6b64d1d" }
            }
        });

        var alice = new Keycloak.User("alice", new Keycloak.UserArgs
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
            Attributes =
            {
                { "organization-id", "84c294a2-ac18-418f-b4e0-d86ce6b64d1d" }
            }
        });
    }
}