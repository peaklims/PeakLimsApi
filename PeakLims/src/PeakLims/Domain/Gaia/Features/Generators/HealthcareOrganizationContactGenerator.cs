namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using Databases;
using HealthcareOrganizationContacts;
using HealthcareOrganizationContacts.Models;
using HealthcareOrganizations;
using Microsoft.EntityFrameworkCore;
using Npis;
using Serilog;

public interface IHealthcareOrganizationContactGenerator
{
    Task<List<HealthcareOrganization>> Generate(Guid organizationId, CancellationToken cancellationToken = default);
}

public class HealthcareOrganizationContactGenerator(PeakLimsDbContext dbContext) : IHealthcareOrganizationContactGenerator
{
    public async Task<List<HealthcareOrganization>> Generate(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var healthcareOrganizations = await dbContext.HealthcareOrganizations
            .Include(x => x.HealthcareOrganizationContacts)
            .Where(x => x.OrganizationId == organizationId)
            .ToListAsync(cancellationToken: cancellationToken);
        
        var existingHealthcareOrgContacts = healthcareOrganizations
            .SelectMany(x => x.HealthcareOrganizationContacts)
            .ToList();
        
        if (existingHealthcareOrgContacts.Count > 0)
        {
            Log.Information("Healthcare Organization Contactss already exist for organization {OrganizationId} -- skipping generation", organizationId);
            return healthcareOrganizations;
        }
        
        var organizationsWithContacts = await GenerateCore(healthcareOrganizations);

        var contactsToAdd = organizationsWithContacts.SelectMany(x => x.HealthcareOrganizationContacts);
        await dbContext.HealthcareOrganizationContacts.AddRangeAsync(contactsToAdd, cancellationToken);
        
        return organizationsWithContacts;
    }
    
    public async Task<List<HealthcareOrganization>> GenerateCore(List<HealthcareOrganization> healthcareOrganizations)
    {
        Log.Information("Starting Healthcare Organization contact creation");
        var orgsToLoop = new ConcurrentBag<HealthcareOrganization>(healthcareOrganizations);
        var orgsWithContacts = new ConcurrentBag<HealthcareOrganization>();
        ValueTask AddContactsToOrgs(HealthcareOrganization org, CancellationToken ct)
        {
            var contacts = GenerateContactsForOrganizationAsync(org).ToList();
            org.AddContacts(contacts);
            orgsWithContacts.Add(org);
            
            return ValueTask.CompletedTask;
        }
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 100
        };
        await Parallel.ForEachAsync(orgsToLoop, options, AddContactsToOrgs);

        Log.Information("Healthcare Organization contacts created: {ContactCount}", orgsWithContacts.Count);
        return orgsWithContacts.ToList();
    }

    private IEnumerable<HealthcareOrganizationContact> GenerateContactsForOrganizationAsync(HealthcareOrganization healthcareOrganization)
    {
        var people = PersonInfoGenerator.Generate();
        
        var contacts = new ConcurrentBag<HealthcareOrganizationContact>();
        foreach (var parsedJsonContact in people)
        {
            var title = new Random().Next(0, 2) == 0 ? "Dr" : null;
            var contactToCreate = new HealthcareOrganizationContactForCreation()
            {
                FirstName = parsedJsonContact.FirstName,
                LastName = parsedJsonContact.LastName,
                Title = title,
                Email = $"{parsedJsonContact.FirstName.ToLower()}.{parsedJsonContact.LastName.ToLower()}@{healthcareOrganization.KnownDomain}",
                Npi = NPI.Random().Value
            };
        
            var contact = HealthcareOrganizationContact.Create(contactToCreate);
            contacts.Add(contact);
        }
        Log.Information("Parsed contacts for Organization {Organization}", healthcareOrganization.Name);

        return contacts;
    }
}