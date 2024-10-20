namespace PeakLims.Services.External.Keycloak.Models;

public sealed class UserRepresentation
{
    // public string Self { get; set; }
    public string Id { get; set; }
    public int CreatedTimestamp { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public bool Enabled { get; set; }
    public bool Totp { get; set; }
    public bool EmailVerified { get; set; }
    public Dictionary<string, ICollection<string>> Attributes { get; set; }
    public CredentialRepresentation[] Credentials { get; set; }
    public string[] RequiredActions { get; set; }
    // public FederatedIdentitiesInfo[] FederatedIdentities { get; set; }
    // public SocialLinksInfo[] SocialLinks { get; set; }
    // public string[] RealmRoles { get; set; }
    // public ClientRolesInfo ClientRoles { get; set; }
    // public ClientConsentsInfo[] ClientConsents { get; set; }
    // public int NotBefore { get; set; }
    // public Dictionary<string, ICollection<string>> ApplicationRoles { get; set; }
    // public string FederationLink { get; set; }
    // public string ServiceAccountClientId { get; set; }
    // public string[] Groups { get; set; }
    // public string Origin { get; set; }
    // public string[] DisableableCredentialTypes { get; set; }
    public Dictionary<string, ICollection<string>> Access { get; set; }

    public sealed class CredentialRepresentation
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string UserLabel { get; set; }
        public string SecretData { get; set; }
        public string CredentialData { get; set; }
        public int Priority { get; set; }
        public int CreatedDate { get; set; }
        public string Value { get; set; }
        public bool Temporary { get; set; }
        public string Device { get; set; }
        public string HashedSaltedValue { get; set; }
        public string Salt { get; set; }
        public int HashIterations { get; set; }
        public int Counter { get; set; }
        public string Algorithm { get; set; }
        public int Digits { get; set; }
        public int Period { get; set; }
        public ConfigInfo Config { get; set; }
    }

    public sealed class ConfigInfo
    {
        public string AdditionalProp1 { get; set; }
        public string AdditionalProp2 { get; set; }
        public string AdditionalProp3 { get; set; }
    }

    public sealed class FederatedIdentitiesInfo
    {
        public string IdentityProvider { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public sealed class SocialLinksInfo
    {
        public string SocialProvider { get; set; }
        public string SocialUserId { get; set; }
        public string SocialUsername { get; set; }
    }

    public sealed class ClientRolesInfo
    {
        public string[] AdditionalProp1 { get; set; }
        public string[] AdditionalProp2 { get; set; }
        public string[] AdditionalProp3 { get; set; }
    }

    public sealed class ClientConsentsInfo
    {
        public string ClientId { get; set; }
        public string[] GrantedClientScopes { get; set; }
        public int CreatedDate { get; set; }
        public int LastUpdatedDate { get; set; }
        public string[] GrantedRealmRoles { get; set; }
    }
}