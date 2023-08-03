namespace PeakLims.Domain.Accessions.Mappings;

using PeakLims.Domain.Accessions.Dtos;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AccessionMapper
{
    public static partial AccessionDto ToAccessionDto(this Accession accession);
    public static partial IQueryable<AccessionDto> ToAccessionDtoQueryable(this IQueryable<Accession> accession);
}