namespace PeakLims.Domain.Gaia.Models;

using System.Text.Json.Serialization;

public record HealthcareOrganizationContactsResponse
{
    // [JsonPropertyName("contacts")]
    public List<OrganizationContact> Contacts { get; set; } = new();

    public record OrganizationContact
    {
        // [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        
        // [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        // [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}