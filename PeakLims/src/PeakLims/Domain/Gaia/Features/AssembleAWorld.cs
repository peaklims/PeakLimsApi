namespace PeakLims.Domain.Gaia.Features;

using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using AccessionComments;
using Databases;
using MediatR;
using Domain.Gaia.Features.Generators;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using PeakOrganizations;
using Resources.HangfireUtilities;
using SampleTypes;
using Serilog;
using WorldBuildingAttempts;
using WorldBuildingPhaseNames;
using WorldBuildingPhases;
using WorldBuildingStatuses;
using Container = Containers.Container;

public static class AssembleAWorld
{
    public sealed record Command(Guid OrganizationId, string SpecialOrganizationRequest) : IRequest<Guid>;
    
    public sealed class Handler(
        IBackgroundJobClient backgroundJobClient,
        IOrganizationGenerator organizationGenerator,
        IUserInfoGenerator userInfoGenerator,
        IHealthcareOrganizationGenerator healthcareOrganizationGenerator,
        IAccessionGenerator accessionGenerator,
        IPanelTestGenerator panelTestGenerator,
        IPatientGenerator patientGenerator,
        IHealthcareOrganizationContactGenerator healthcareOrganizationContactGenerator,
        PeakLimsDbContext dbContext
    ) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var worldBuildingAttempt = WorldBuildingAttempt.CreateStandardWorld();
            dbContext.WorldBuildingAttempts.Add(worldBuildingAttempt);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            var command = new GenerateWorldJobCommand(worldBuildingAttempt.Id, request.OrganizationId, request.SpecialOrganizationRequest);
            
            backgroundJobClient.Enqueue(() => GenerateWorldJob(command, cancellationToken));
            
            return worldBuildingAttempt.Id;
        }

        public sealed record GenerateWorldJobCommand(
            Guid WorldBuildingAttemptId,
            Guid OrganizationId,
            string SpecialOrganizationRequest);
        
        [JobUserFilter]
        [DisplayName("Assemble A World")]
        [AutomaticRetry(Attempts = 3)]
        public async Task GenerateWorldJob(GenerateWorldJobCommand command, CancellationToken cancellationToken)
        {
            var worldBuildingAttempt = await dbContext.WorldBuildingAttempts
                .Include(x => x.WorldBuildingPhases)
                .GetById(command.WorldBuildingAttemptId, cancellationToken: cancellationToken);
            
            var organizationId = command.OrganizationId;
            var organization = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.CreateOrganization(),
                () => organizationGenerator.Generate(organizationId, command.SpecialOrganizationRequest, cancellationToken),
                command.SpecialOrganizationRequest,
                cancellationToken
            );
            
            await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateUsers(),
                () => userInfoGenerator.Generate(organizationId, cancellationToken),
                null,
                cancellationToken
            );

            await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateHealthcareOrganizations(),
                () => healthcareOrganizationGenerator.Generate(organizationId, cancellationToken),
                null,
                cancellationToken
            );

            await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateHealthcareOrganizationContacts(),
                () => healthcareOrganizationContactGenerator.Generate(organizationId, cancellationToken),
                null,
                cancellationToken
            );

            await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GeneratePanelsAndTests(),
                () => panelTestGenerator.Generate(organizationId, cancellationToken),
                null,
                cancellationToken
            );

            await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.AddDefaultContainers(),
                () => AddDefaultContainers(organization, cancellationToken),
                null,
                cancellationToken
            );

            await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GeneratePatients(),
                () => patientGenerator.Generate(organizationId),
                null,
                cancellationToken
            );

            await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateAccessions(),
                () => accessionGenerator.Generate(organizationId, cancellationToken),
                null,
                cancellationToken
            );
        }

        private async Task<T> ExecutePhaseAsync<T>(WorldBuildingAttempt worldBuildingAttempt,
            WorldBuildingPhaseName phaseName,
            Func<Task<T>> action,
            string specialRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                worldBuildingAttempt.StartPhase(phaseName, specialRequest);
                await dbContext.SaveChangesAsync(cancellationToken);

                var result = await action();

                worldBuildingAttempt.SuccessfullyEndPhase(phaseName, result);
                await dbContext.SaveChangesAsync(cancellationToken);

                return result;
            }
            catch (Exception e)
            {
                worldBuildingAttempt.FailPhase(phaseName, e.Message);
                await dbContext.SaveChangesAsync(cancellationToken);
                throw;
            }
        }

        private async Task<List<Container>> AddDefaultContainers(PeakOrganization organization, CancellationToken cancellationToken = default)
        {
            var existingContainers = await dbContext.Containers
                .Where(x => x.OrganizationId == organization.Id)
                .ToListAsync(cancellationToken: cancellationToken);
            
            if (existingContainers.Count > 0)
            {
                Log.Information("Containers already exist for organization {OrganizationId} -- skipping generation", organization.Id);
                return existingContainers;
            }
            
            var containerList = new List<Container>();
            foreach (var sampleTypeName in SampleType.ListNames())
            {
                var sampleType = SampleType.Of(sampleTypeName);
                var defaultContainers = sampleType.GetDefaultContainers(organization.Id);
                containerList.AddRange(defaultContainers);
                
                await dbContext.Containers.AddRangeAsync(defaultContainers, cancellationToken);
            }
            return containerList;
        }
    }
}