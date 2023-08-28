namespace PeakLims.Domain.AccessionContacts.Mappings;

using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AccessionContactMapper
{
    public static partial AccessionContactForCreation ToAccessionContactForCreation(this AccessionContactForCreationDto accessionContactForCreationDto);
    public static partial AccessionContactForUpdate ToAccessionContactForUpdate(this AccessionContactForUpdateDto accessionContactForUpdateDto);
    
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.FirstName) }, new[] { nameof(AccessionContactDto.FirstName) })]
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.LastName) }, new[] { nameof(AccessionContactDto.LastName) })]
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.Npi) }, new[] { nameof(AccessionContactDto.Npi) })]
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.Id) }, new[] { nameof(AccessionContactDto.OrganizationContactId) })]
    public static partial AccessionContactDto ToAccessionContactDto(this AccessionContact accessionContact);
    
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.FirstName) }, new[] { nameof(AccessionContactDto.FirstName) })]
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.LastName) }, new[] { nameof(AccessionContactDto.LastName) })]
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.Npi) }, new[] { nameof(AccessionContactDto.Npi) })]
    [MapProperty(new[] { nameof(AccessionContact.HealthcareOrganizationContact), nameof(AccessionContact.HealthcareOrganizationContact.Id) }, new[] { nameof(AccessionContactDto.OrganizationContactId) })]
    public static partial IQueryable<AccessionContactDto> ToAccessionContactDtoQueryable(this IQueryable<AccessionContact> accessionContact);
}