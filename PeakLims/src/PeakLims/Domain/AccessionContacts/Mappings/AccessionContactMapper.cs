namespace PeakLims.Domain.AccessionContacts.Mappings;

using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AccessionContactMapper
{
    public static partial AccessionContactForCreation ToAccessionContactForCreation(this AccessionContactForCreationDto accessionContactForCreationDto);
    public static partial AccessionContactForUpdate ToAccessionContactForUpdate(this AccessionContactForUpdateDto accessionContactForUpdateDto);
    public static partial AccessionContactDto ToAccessionContactDto(this AccessionContact accessionContact);
    public static partial IQueryable<AccessionContactDto> ToAccessionContactDtoQueryable(this IQueryable<AccessionContact> accessionContact);
}