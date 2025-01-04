namespace PeakLims.Domain.Gaia.Features;

using System.Threading.Tasks;
using System.Threading;
using Containers;
using Databases;
using MediatR;
using Domain.Gaia.Features.Generators;
using Domain.HealthcareOrganizations;
using Domain.HealthcareOrganizations.Models;
using Microsoft.Extensions.AI;
using Panels;
using Panels.Models;
using SampleTypes;
using Serilog;
using Services.External.Keycloak;
using Tests;
using Tests.Models;


// TODO refactor to HF job or temporal workflow
public static class AssembleAWorld
{
    public sealed record Command() : IRequest<object>;
    
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
            var organization = await organizationGenerator.Generate();
            await dbContext.PeakOrganizations.AddAsync(organization, cancellationToken);
            // TODO save phase
            
            var users = await userInfoGenerator.Generate(organization.Id, organization.Domain);
            // TODO save phase
            
            var healthcareOrganizations = await healthcareOrganizationGenerator.Generate(organization);
            // TODO save phase
        
            var healthcareOrgsWithContacts = await new HealthcareOrganizationContactGenerator()
                .Generate(healthcareOrganizations);
            await dbContext.HealthcareOrganizations.AddRangeAsync(healthcareOrgsWithContacts, cancellationToken);
            // TODO save phase
            
            // ----------------- Add Default Containers -----------------
            
            var containerList = new List<Container>();
            foreach (var sampleTypeName in SampleType.ListNames())
            {
                var sampleType = SampleType.Of(sampleTypeName);
                var defaultContainers = sampleType.GetDefaultContainers(organization.Id);
                containerList.AddRange(defaultContainers);
            }
            await dbContext.Containers.AddRangeAsync(containerList, cancellationToken);
            
            // -----------------------------------------------------------
            
            var panelsAndTests = await panelTestGenerator.Generate(organization.Id);
            
            var patients = await PatientGenerator.Generate(organization.Id, containerList);
            // TODO save phase

            var accessions =
                await accessionGenerator.Generate(patients, healthcareOrganizations, panelsAndTests, users);
            await dbContext.Accessions.AddRangeAsync(accessions, cancellationToken);
            
            // TODO save phase
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
    }

}