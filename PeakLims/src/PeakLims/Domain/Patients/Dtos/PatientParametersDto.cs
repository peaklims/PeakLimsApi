namespace PeakLims.Domain.Patients.Dtos;

using Resources;

public sealed class PatientParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
