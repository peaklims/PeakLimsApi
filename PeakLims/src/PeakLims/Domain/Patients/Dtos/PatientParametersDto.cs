namespace PeakLims.Domain.Patients.Dtos;

using PeakLims.Dtos;

public sealed class PatientParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
