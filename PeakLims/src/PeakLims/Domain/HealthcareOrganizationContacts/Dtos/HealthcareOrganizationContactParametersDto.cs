namespace PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

using Resources;

public sealed class HealthcareOrganizationContactParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
