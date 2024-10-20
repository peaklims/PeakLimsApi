namespace PeakLims.Domain.Gaia.Features;

using System.Threading.Tasks;
using System.Threading;
using Databases;
using MediatR;
using Domain.Gaia.Features.Generators;
using Domain.HealthcareOrganizations;
using Domain.HealthcareOrganizations.Models;
using Microsoft.Extensions.AI;
using Serilog;
using Services.External.Keycloak;

public static class AssembleAWorld
{
    public sealed record Command() : IRequest<object>;
    
    public sealed class Handler(IChatClient chatClient, 
        IOrganizationGenerator organizationGenerator,
        IUserInfoGenerator userInfoGenerator,
        IHealthcareOrganizationGenerator healthcareOrganizationGenerator,
        IAccessionGenerator accessionGenerator,
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
            
            var patients = await PatientGenerator.Generate(organization.Id);
            // TODO save phase
            
            var accessions = await accessionGenerator.Generate(patients, healthcareOrganizations);
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