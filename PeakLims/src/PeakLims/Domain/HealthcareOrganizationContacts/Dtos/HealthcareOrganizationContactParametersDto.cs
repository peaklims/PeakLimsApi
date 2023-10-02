namespace PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

using PeakLims.Dtos;

public sealed class HealthcareOrganizationContactParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
