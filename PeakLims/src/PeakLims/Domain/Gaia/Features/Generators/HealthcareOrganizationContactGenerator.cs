namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using HealthcareOrganizationContacts;
using HealthcareOrganizationContacts.Models;
using HealthcareOrganizations;
using Npis;
using Serilog;

public class HealthcareOrganizationContactGenerator()
{
    public async Task<List<HealthcareOrganization>> Generate(List<HealthcareOrganization> healthcareOrganizations)
    {
        Log.Information("Starting Healthcare Organization contact creation");
        var orgsToLoop = new ConcurrentBag<HealthcareOrganization>(healthcareOrganizations);
        var orgsWithContacts = new ConcurrentBag<HealthcareOrganization>();
        ValueTask AddContactsToOrgs(HealthcareOrganization org, CancellationToken ct)
        {
            var contacts = GenerateContactsForOrganizationAsync(org);
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

    private IEnumerable<HealthcareOrganizationContact> GenerateContactsForOrganizationAsync(HealthcareOrganization organization)
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
                Email = $"{parsedJsonContact.FirstName.ToLower()}.{parsedJsonContact.LastName.ToLower()}@{organization.KnownDomain}",
                Npi = NPI.Random().Value
            };
        
            var contact = HealthcareOrganizationContact.Create(contactToCreate);
            contacts.Add(contact);
        }
        Log.Information("Parsed contacts for Organization {Organization}", organization.Name);

        return contacts;
    }
}