namespace PeakLims.Domain.AccessionComments.Features;

using AccessionCommentStatuses;
using Accessions;
using Accessions.Services;
using Databases;
using Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Services;
using Users;

public static class GetAccessionCommentView
{
    public sealed record Query(Guid AccessionId) : IRequest<AccessionCommentViewDto>
    {
        public readonly Guid AccessionId = AccessionId;
    }

    public sealed class Handler(IAccessionRepository accessionRepository,
            PeakLimsDbContext dbContext,
            ICurrentUserService currentUserService)
        : IRequestHandler<Query, AccessionCommentViewDto>
    {
        public async Task<AccessionCommentViewDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var accession = await dbContext.Accessions
                .Include(x => x.Comments)
                .ThenInclude(x => x.ParentComment)
                .AsNoTracking()
                .GetById(request.AccessionId, cancellationToken);

            var commentView = new AccessionCommentViewDto();

            var allAccessionComments = accession.Comments.ToList();
            var activeAccessionComments = allAccessionComments
                .Where(x => x.Status == AccessionCommentStatus.Active())
                .OrderBy(x => x.CommentedAt)
                .ToList();
            var distinctAccessionCommentUserIdList = allAccessionComments.Select(x => x.CommentedByIdentifier)
                .Distinct()
                .ToList();

            var distinctUserList = await dbContext.Users
                .AsNoTracking()
                .Where(x => distinctAccessionCommentUserIdList.Contains(x.Identifier))
                .ToListAsync(cancellationToken);

            var currentUserId = currentUserService.UserIdentifier;
            foreach (var accessionComment in activeAccessionComments)
            {
                commentView.AccessionComments.Add(GetAccessionCommentItemDto(accessionComment, allAccessionComments, distinctUserList, currentUserId));
            }

            commentView.AccessionComments =
                commentView.AccessionComments
                    .OrderBy(x => x.OriginalCommentAt)
                    .ToList();

            return commentView;
        }
    }
    
    private static AccessionCommentViewDto.AccessionCommentItemDto GetAccessionCommentItemDto(
        AccessionComment accessionComment,
        IList<AccessionComment> allAccessionComments,
        List<User> distinctUserList, string currentUserId)
    {
        var owner = distinctUserList.FirstOrDefault(x => x.Identifier == accessionComment.CommentedByIdentifier);
        var isUnknownUser = owner == null;
        var accessionCommentDto = new AccessionCommentViewDto.AccessionCommentItemDto
        {
            Id = accessionComment.Id,
            Comment = accessionComment.Comment,
            CreatedDate = accessionComment.CommentedAt.DateTime,
            CreatedByFirstName = isUnknownUser ? "Unknown" : owner?.FirstName,
            CreatedByLastName = owner?.LastName,
            CreatedById = accessionComment?.CommentedByIdentifier,
            OwnedByCurrentUser = accessionComment.CommentedByIdentifier == currentUserId,
            History = new List<AccessionCommentViewDto.AccessionCommentHistoryRecordDto>()
        };

        var archivedAccessionComments = allAccessionComments
            .Where(tsi => tsi.Status == AccessionCommentStatus.Archived() && 
                          tsi.ParentComment != null && 
                          tsi.ParentComment.Id == accessionComment.Id);

        var accessionCommentHistory = new List<AccessionCommentViewDto.AccessionCommentHistoryRecordDto>();

        var historyStack = new Stack<AccessionComment>(archivedAccessionComments);
        while (historyStack.Count > 0)
        {
            var archivedAccessionComment = historyStack.Pop();
            var archivedOwner = distinctUserList.FirstOrDefault(x => x.Identifier == archivedAccessionComment.CommentedByIdentifier);
            var isUnknownArchivedUser = archivedOwner == null;
            accessionCommentHistory.Add(new AccessionCommentViewDto.AccessionCommentHistoryRecordDto
            {
                Id = archivedAccessionComment.Id,
                Comment = archivedAccessionComment.Comment,
                CreatedDate = archivedAccessionComment.CommentedAt.DateTime,
                CreatedByFirstName = isUnknownArchivedUser ? "Unknown" : archivedOwner?.FirstName,
                CreatedByLastName = archivedOwner?.LastName,
                CreatedById = archivedAccessionComment?.CommentedByIdentifier,
                OwnedByCurrentUser = accessionComment.CommentedByIdentifier == currentUserId,
            });

            var childArchivedAccessionComments = allAccessionComments
                .Where(tsi => tsi?.Status == AccessionCommentStatus.Archived() && 
                              tsi?.ParentComment?.Id == archivedAccessionComment.Id)
                .OrderBy(x => x?.CommentedAt);
            foreach (var childArchivedAccessionComment in childArchivedAccessionComments)
            {
                historyStack.Push(childArchivedAccessionComment);
            }
        }

        accessionCommentDto.History.AddRange(accessionCommentHistory);
        accessionCommentDto.OriginalCommentAt = accessionCommentDto.History.LastOrDefault()?.CreatedDate 
                                                ?? accessionCommentDto.CreatedDate;

        return accessionCommentDto;
    }
}
