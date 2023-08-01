namespace PeakLims.Domain.AccessionComments.Features;

using Ardalis.Specification;
using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Domain.AccessionComments.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetAccessionCommentList
{
    public sealed class Query : IRequest<PagedList<AccessionCommentDto>>
    {
        public readonly AccessionCommentParametersDto QueryParameters;

        public Query(AccessionCommentParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<AccessionCommentDto>>
    {
        private readonly IAccessionCommentRepository _accessionCommentRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionCommentRepository accessionCommentRepository, IHeimGuardClient heimGuard)
        {
            _accessionCommentRepository = accessionCommentRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<AccessionCommentDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessionComments);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var accessionCommentSpecification = new AccessionCommentWorklistSpecification(request.QueryParameters, queryKitData);
            var accessionCommentList = await _accessionCommentRepository.ListAsync(accessionCommentSpecification, cancellationToken);
            var totalAccessionCommentCount = await _accessionCommentRepository.TotalCount(cancellationToken);

            return new PagedList<AccessionCommentDto>(accessionCommentList, 
                totalAccessionCommentCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class AccessionCommentWorklistSpecification : Specification<AccessionComment, AccessionCommentDto>
        {
            public AccessionCommentWorklistSpecification(AccessionCommentParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToAccessionCommentDto())
                    .AsNoTracking();
            }
        }
    }
}