namespace PeakLims.Domain.AccessionContacts.Dtos;

public sealed record AccessionContactDto
{
    public Guid Id { get; set; }
    public string TargetType { get; set; }
    public string TargetValue { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Npi { get; set; } = null!;
    public Guid OrganizationContactId { get; set; }
}
