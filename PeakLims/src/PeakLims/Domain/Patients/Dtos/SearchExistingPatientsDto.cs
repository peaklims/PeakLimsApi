namespace PeakLims.Domain.Patients.Dtos;

using Resources;

public sealed class SearchExistingPatientsDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
