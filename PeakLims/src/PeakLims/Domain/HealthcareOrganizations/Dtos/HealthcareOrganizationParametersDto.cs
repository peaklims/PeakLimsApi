namespace PeakLims.Domain.HealthcareOrganizations.Dtos;

using PeakLims.Dtos;

public sealed class HealthcareOrganizationParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
