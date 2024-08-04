namespace PeakLims.Domain.AccessionAttachments.Features;

using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Services;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using PeakLims.Services.External;

public static class GetAccessionAttachment
{
    public sealed record Query(Guid AccessionAttachmentId) : IRequest<AccessionAttachmentDto>;

    public sealed class Handler : IRequestHandler<Query, AccessionAttachmentDto>
    {
        private readonly IAccessionAttachmentRepository _accessionAttachmentRepository;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IFileStorage _fileStorage;

        public Handler(IAccessionAttachmentRepository accessionAttachmentRepository, IHeimGuardClient heimGuard, IFileStorage fileStorage)
        {
            _accessionAttachmentRepository = accessionAttachmentRepository;
            _heimGuard = heimGuard;
            _fileStorage = fileStorage;
        }

        public async Task<AccessionAttachmentDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _accessionAttachmentRepository.GetById(request.AccessionAttachmentId, cancellationToken: cancellationToken);
            return result.ToAccessionAttachmentDto(_fileStorage);
        }
    }
}