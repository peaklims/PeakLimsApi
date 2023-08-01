namespace PeakLims.Domain.HealthcareOrganizations.Dtos;

using SharedKernel.Dtos;

public sealed class HealthcareOrganizationParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
