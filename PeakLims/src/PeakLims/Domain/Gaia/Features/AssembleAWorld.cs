namespace PeakLims.Domain.Gaia.Features;

using System.Threading.Tasks;
using System.Threading;
using Containers;
using Databases;
using MediatR;
using Domain.Gaia.Features.Generators;
using PeakOrganizations;
using SampleTypes;
using WorldBuildingAttempts;
using WorldBuildingPhaseNames;

// TODO refactor to HF job or temporal workflow
public static class AssembleAWorld
{
    public sealed record Command(string SpecialOrganizationRequest) : IRequest<object>;
    
    public sealed class Handler(
        IOrganizationGenerator organizationGenerator,
        IUserInfoGenerator userInfoGenerator,
        IHealthcareOrganizationGenerator healthcareOrganizationGenerator,
        IAccessionGenerator accessionGenerator,
        IPanelTestGenerator panelTestGenerator,
        PeakLimsDbContext dbContext
    ) : IRequestHandler<Command, object>
    {
        public async Task<object> Handle(Command request, CancellationToken cancellationToken)
        {
            var worldBuildingAttempt = WorldBuildingAttempt.CreateStandardWorld();
            dbContext.WorldBuildingAttempts.Add(worldBuildingAttempt);
            
            var organization = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.CreateOrganization(),
                () => organizationGenerator.Generate(request.SpecialOrganizationRequest),
                request.SpecialOrganizationRequest,
                cancellationToken
            );
            
            var users = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateUsers(),
                () => userInfoGenerator.Generate(organization.Id, organization.Domain),
                null,
                cancellationToken
            );

            var healthcareOrganizations = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateHealthcareOrganizations(),
                () => healthcareOrganizationGenerator.Generate(organization),
                null,
                cancellationToken
            );

            var healthcareOrgsWithContacts = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateHealthcareOrganizationContacts(),
                () => new HealthcareOrganizationContactGenerator().Generate(healthcareOrganizations),
                null,
                cancellationToken
            );

            var panelsAndTests = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GeneratePanelsAndTests(),
                () => panelTestGenerator.Generate(organization.Id),
                null,
                cancellationToken
            );

            var containerList = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.AddDefaultContainers(),
                () => AddDefaultContainers(organization),
                null,
                cancellationToken
            );

            var patients = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GeneratePatients(),
                () => PatientGenerator.Generate(organization.Id, containerList),
                null,
                cancellationToken
            );

            var accessions = await ExecutePhaseAsync(
                worldBuildingAttempt,
                WorldBuildingPhaseName.GenerateAccessions(),
                () => accessionGenerator.Generate(patients, healthcareOrganizations, panelsAndTests, users),
                null,
                cancellationToken
            );

            
            worldBuildingAttempt.StartPhase(WorldBuildingPhaseName.FinalizeInDatabase(), null);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.PeakOrganizations.AddAsync(organization, cancellationToken);
            await dbContext.Users.AddRangeAsync(users, cancellationToken);
            await dbContext.HealthcareOrganizations.AddRangeAsync(healthcareOrgsWithContacts, cancellationToken);
            
            await dbContext.Panels.AddRangeAsync(panelsAndTests.Panels, cancellationToken);
            await dbContext.Tests.AddRangeAsync(panelsAndTests.StandaloneTests, cancellationToken);
            await dbContext.Containers.AddRangeAsync(containerList, cancellationToken);
            
            await dbContext.Accessions.AddRangeAsync(accessions, cancellationToken);
            
            worldBuildingAttempt.SuccessfullyEndPhase(WorldBuildingPhaseName.FinalizeInDatabase(), null);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return new
            {
                Organization = new
                {
                    organization.Name,
                    organization.Domain
                },
                Users = users.Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    x.Username
                }).ToList(),
                Accessions = accessions.Select(x => new
                {
                    x.AccessionNumber,
                    Patient = new
                    {
                        x.Patient.FirstName,
                        x.Patient.LastName,
                        Sex = x.Patient.Sex.Value,
                        x.Patient.Lifespan.Age,
                        x.Patient.Lifespan.DateOfBirth,
                        Race = x.Patient.Race.Value,
                        Ethnicity = x.Patient.Ethnicity.Value
                    },
                    Org = x.HealthcareOrganization.Name,
                    Contacts = x.AccessionContacts.Select(c => new
                    {
                        c.HealthcareOrganizationContact.FirstName,
                        c.HealthcareOrganizationContact.LastName,
                        c.HealthcareOrganizationContact.Email,
                        Npi = c.HealthcareOrganizationContact.Npi.Value
                    }).ToList()
                }).ToList(),
            };
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

        private async Task<List<Container>> AddDefaultContainers(PeakOrganization organization)
        {
            var containerList = new List<Container>();
            foreach (var sampleTypeName in SampleType.ListNames())
            {
                var sampleType = SampleType.Of(sampleTypeName);
                var defaultContainers = sampleType.GetDefaultContainers(organization.Id);
                containerList.AddRange(defaultContainers);
            }
            return containerList;
        }
    }

}