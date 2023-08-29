namespace PeakLims.Domain.Samples.Mappings;

using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class SampleMapper
{
    public static partial SampleForCreation ToSampleForCreation(this SampleForCreationDto sampleForCreationDto);
    public static partial SampleForUpdate ToSampleForUpdate(this SampleForUpdateDto sampleForUpdateDto);
    
    [MapProperty(new[] { nameof(Sample.Patient), nameof(Sample.Patient.Id) }, new[] { nameof(SampleDto.PatientId) })]
    public static partial SampleDto ToSampleDto(this Sample sample);
    
    [MapProperty(new[] { nameof(Sample.Patient), nameof(Sample.Patient.Id) }, new[] { nameof(SampleDto.PatientId) })]
    public static partial IQueryable<SampleDto> ToSampleDtoQueryable(this IQueryable<Sample> sample);
}