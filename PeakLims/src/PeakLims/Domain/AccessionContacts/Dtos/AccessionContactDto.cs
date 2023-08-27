namespace PeakLims.Domain.AccessionContacts.Dtos;

public sealed record AccessionContactDto
{
    public Guid Id { get; set; }
    public string TargetType { get; set; }
    public string TargetValue { get; set; }

}
