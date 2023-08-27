namespace PeakLims.Domain.AccessionContacts.Dtos;

public sealed record AccessionContactForUpdateDto
{
    public string TargetType { get; set; }
    public string TargetValue { get; set; }

}
