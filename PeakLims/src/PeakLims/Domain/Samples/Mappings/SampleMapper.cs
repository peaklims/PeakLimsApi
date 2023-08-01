namespace PeakLims.Domain.Samples.Mappings;

using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class SampleMapper
{
    public static partial SampleForCreation ToSampleForCreation(this SampleForCreationDto sampleForCreationDto);
    public static partial SampleForUpdate ToSampleForUpdate(this SampleForUpdateDto sampleForUpdateDto);
    public static partial SampleDto ToSampleDto(this Sample sample);
    public static partial IQueryable<SampleDto> ToSampleDtoQueryable(this IQueryable<Sample> sample);
}