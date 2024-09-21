namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using Patients.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using Patients;
using Patients.Dtos;
using Patients.Mappings;

public static class SetAccessionPatient
{
    public sealed record Command(Guid AccessionId, Guid? PatientId, PatientForCreationDto PatientForCreationDto) : IRequest;

    public sealed class Handler(
        PeakLimsDbContext dbContext, 
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard,
        IPatientRepository patientRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = (await dbContext.GetAccessionAggregate()
                .GetById(request.AccessionId, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            var isNewPatient = request.PatientId == null;
            if (isNewPatient)
            {
                if (request.PatientForCreationDto == null)
                    throw new ArgumentNullException(nameof(request.PatientForCreationDto));
                
                var newPatientToCreate = request.PatientForCreationDto.ToPatientForCreation();
                var newPatient = Patient.Create(newPatientToCreate);
                await patientRepository.Add(newPatient, cancellationToken);
                accession.SetPatient(newPatient);
            }
            else
            {
                var existingPatient = await patientRepository.GetById(request.PatientId!.Value, cancellationToken: cancellationToken);
                accession.SetPatient(existingPatient);
            }

            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}