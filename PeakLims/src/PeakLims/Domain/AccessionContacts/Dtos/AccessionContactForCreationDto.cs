namespace PeakLims.Domain.AccessionContacts.Dtos;

public sealed record AccessionContactForCreationDto
{
    public string TargetType { get; set; }
    public string TargetValue { get; set; }

}
