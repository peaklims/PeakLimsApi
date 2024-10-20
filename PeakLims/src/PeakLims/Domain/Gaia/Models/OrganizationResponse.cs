namespace PeakLims.Domain.Gaia.Models;

using System.Text.Json.Serialization;

public record OrganizationResponse
{
    public List<OrganizationRecord> Organizations { get; set; } = new();

    public record OrganizationRecord
    {
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }
}