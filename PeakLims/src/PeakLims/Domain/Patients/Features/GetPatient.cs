namespace PeakLims.Domain.Patients.Features;

using Databases;
using PeakLims.Domain.Patients.Dtos;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetPatient
{
    public sealed record Query(Guid Id) : IRequest<PatientDto>;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Query, PatientDto>
    {
        public async Task<PatientDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await dbContext.Patients.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToPatientDto();
        }
    }
}