namespace PeakLims.Domain.AccessionContacts;

using SharedKernel.Exceptions;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.AccessionContacts.Models;
using PeakLims.Domain.AccessionContacts.DomainEvents;

public class AccessionContact : BaseEntity
{
    private TargetTypeEnum _targetType;
    public string TargetType
    {
        get => _targetType.Name;
        private set
        {
            if (!TargetTypeEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(TargetType), value);

            _targetType = parsed;
        }
    }

    public string TargetValue { get; private set; }

    public Accession Accession { get; }

    public HealthcareOrganizationContact HealthcareOrganizationContact { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static AccessionContact Create(AccessionContactForCreation accessionContactForCreation)
    {
        var newAccessionContact = new AccessionContact();

        newAccessionContact.TargetType = accessionContactForCreation.TargetType;
        newAccessionContact.TargetValue = accessionContactForCreation.TargetValue;

        newAccessionContact.QueueDomainEvent(new AccessionContactCreated(){ AccessionContact = newAccessionContact });
        
        return newAccessionContact;
    }

    public AccessionContact Update(AccessionContactForUpdate accessionContactForUpdate)
    {
        TargetType = accessionContactForUpdate.TargetType;
        TargetValue = accessionContactForUpdate.TargetValue;

        QueueDomainEvent(new AccessionContactUpdated(){ Id = Id });
        return this;
    }

    public AccessionContact SetHealthcareOrganizationContact(HealthcareOrganizationContact healthcareOrganizationContact)
    {
        HealthcareOrganizationContact = healthcareOrganizationContact;
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected AccessionContact() { } // For EF + Mocking
}
