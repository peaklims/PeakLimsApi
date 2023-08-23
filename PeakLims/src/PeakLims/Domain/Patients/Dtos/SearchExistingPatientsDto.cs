namespace PeakLims.Domain.Patients.Dtos;

using SharedKernel.Dtos;

public sealed class SearchExistingPatientsDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
