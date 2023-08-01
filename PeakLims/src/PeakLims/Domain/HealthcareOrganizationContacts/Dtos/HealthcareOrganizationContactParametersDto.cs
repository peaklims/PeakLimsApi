namespace PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

using SharedKernel.Dtos;

public sealed class HealthcareOrganizationContactParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
