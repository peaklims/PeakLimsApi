namespace PeakLims.Domain.AccessionComments.Mappings;

using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Domain.AccessionComments.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AccessionCommentMapper
{
    public static partial AccessionCommentForCreation ToAccessionCommentForCreation(this AccessionCommentForCreationDto accessionCommentForCreationDto);
    public static partial AccessionCommentForUpdate ToAccessionCommentForUpdate(this AccessionCommentForUpdateDto accessionCommentForUpdateDto);
    public static partial AccessionCommentDto ToAccessionCommentDto(this AccessionComment accessionComment);
    public static partial IQueryable<AccessionCommentDto> ToAccessionCommentDtoQueryable(this IQueryable<AccessionComment> accessionComment);
}