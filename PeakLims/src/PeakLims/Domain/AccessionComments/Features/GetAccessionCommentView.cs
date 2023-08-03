namespace PeakLims.Domain.AccessionComments.Features;

using AccessionCommentStatuses;
using Accessions;
using Accessions.Services;
using Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Services;
using Users;
using Users.Dtos;
using Users.Services;

public static class GetAccessionCommentView
{
    public sealed class Query : IRequest<AccessionCommentViewDto>
    {
        public readonly Guid AccessionId;

        public Query(Guid accessionId)
        {
            AccessionId = accessionId;
        }
    }

    public sealed class Handler : IRequestHandler<Query, AccessionCommentViewDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccessionRepository _accessionRepository;

        public Handler(IUserRepository userRepository, IAccessionRepository accessionRepository)
        {
            _userRepository = userRepository;
            _accessionRepository = accessionRepository;
        }

        public async Task<AccessionCommentViewDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var accession = await _accessionRepository.Query()
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken);
            
            if (accession == null)
                throw new KeyNotFoundException($"Accession with id {request.AccessionId} not found");

            var treatmentPlanDto = new AccessionCommentViewDto();

            var allAccessionComments = accession.Comments.ToList();
            var activeAccessionComments = allAccessionComments
                .Where(tsi => tsi.Status == AccessionCommentStatus.Active())
                .OrderBy(x => x.CreatedOn)
                .ToList();
            var distinctAccessionCommentUserIdList = allAccessionComments.Select(x => x.CreatedBy)
                .Distinct()
                .ToList();

            var distinctUserList = await _userRepository.Query()
                .Where(x => distinctAccessionCommentUserIdList.Contains(x.CreatedBy))
                .ToListAsync(cancellationToken);
            foreach (var accessionComment in activeAccessionComments)
            {
                treatmentPlanDto.AccessionComments.Add(GetAccessionCommentItemDto(accessionComment, allAccessionComments, distinctUserList));
            }

            return treatmentPlanDto;
        }
    }
    
    private static AccessionCommentViewDto.AccessionCommentItemDto GetAccessionCommentItemDto(AccessionComment accessionComment, IList<AccessionComment> allAccessionComments, List<User> distinctUserList)
    {
        var owner = distinctUserList.FirstOrDefault(x => x.Identifier == accessionComment.CreatedBy);
        var isUnknownUser = owner == null;
        var accessionCommentDto = new AccessionCommentViewDto.AccessionCommentItemDto
        {
            Id = accessionComment.Id,
            Comment = accessionComment.Comment,
            CreatedDate = accessionComment.CreatedOn,
            CreatedByFirstName = isUnknownUser ? "Unknown" : owner?.FirstName,
            CreatedByLastName = owner?.LastName,
            CreatedById = accessionComment?.CreatedBy,
            History = new List<AccessionCommentViewDto.AccessionCommentHistoryRecordDto>()
        };

        var archivedAccessionComments = allAccessionComments
            .Where(tsi => tsi.Status == AccessionCommentStatus.Archived() && 
                          tsi.ParentComment.Id == accessionComment.Id);

        var accessionCommentHistory = new List<AccessionCommentViewDto.AccessionCommentHistoryRecordDto>();

        var historyStack = new Stack<AccessionComment>(archivedAccessionComments);
        while (historyStack.Count > 0)
        {
            var archivedAccessionComment = historyStack.Pop();
            var archivedOwner = distinctUserList.FirstOrDefault(x => x.Identifier == archivedAccessionComment.CreatedBy);
            var isUnknownArchivedUser = archivedOwner == null;
            accessionCommentHistory.Add(new AccessionCommentViewDto.AccessionCommentHistoryRecordDto
            {
                Id = archivedAccessionComment.Id,
                Comment = archivedAccessionComment.Comment,
                CreatedDate = archivedAccessionComment.CreatedOn,
                CreatedByFirstName = isUnknownArchivedUser ? "Unknown" : archivedOwner?.FirstName,
                CreatedByLastName = archivedOwner?.LastName,
                CreatedById = archivedAccessionComment?.CreatedBy
            });

            var childArchivedAccessionComments = allAccessionComments
                .Where(tsi => tsi.Status == AccessionCommentStatus.Archived() && 
                              tsi.ParentComment.Id == archivedAccessionComment.Id)
                .OrderBy(x => x.CreatedOn);
            foreach (var childArchivedAccessionComment in childArchivedAccessionComments)
            {
                historyStack.Push(childArchivedAccessionComment);
            }
        }

        accessionCommentDto.History.AddRange(accessionCommentHistory);

        return accessionCommentDto;
    }
}
