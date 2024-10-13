namespace PeakLims.Domain.HealthcareOrganizations.Dtos;

using Resources;

public sealed class HealthcareOrganizationParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
