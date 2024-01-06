namespace PeakLims.Domain.Accessions.Mappings;

using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Services.External;
using Riok.Mapperly.Abstractions;
using TestOrders;

[Mapper]
public static partial class AccessionMapper
{
    public static partial AccessionDto ToAccessionDto(this Accession accession);
    public static partial IQueryable<AccessionDto> ToAccessionDtoQueryable(this IQueryable<Accession> accession);


    // [MapProperty(new[] { nameof(Accession.Patient.FirstName) }, new[] { nameof(AccessionWorklistDto.Patient.FirstName) })]
    // [MapProperty(new[] { nameof(Accession.Patient.LastName) }, new[] { nameof(AccessionWorklistDto.Patient.LastName) })]
    // [MapProperty(new[] { nameof(Accession.Patient.Lifespan.Age) }, new[] { nameof(AccessionWorklistDto.Patient.Age) })]
    // [MapProperty(new[] { nameof(Accession.TestOrders), nameof(TestOrder.Test), nameof(TestOrder.Test.TestName) }, new[] { nameof(AccessionWorklistDto.TestOrders), nameof(AccessionWorklistDto.TestOrderDto.TestName) })]
    // [MapProperty(new[] { nameof(Accession.TestOrders), nameof(TestOrder.AssociatedPanel), nameof(TestOrder.AssociatedPanel.PanelName) }, new[] { nameof(AccessionWorklistDto.TestOrders), nameof(AccessionWorklistDto.TestOrderDto.PanelName) })]
    
    // [MapProperty(nameof(Accession), nameof(AccessionWorklistDto))]
    // [MapProperty(nameof(Accession.Patient.Lifespan.Age), nameof(AccessionWorklistDto.Patient.Age))]
    public static IQueryable<AccessionWorklistDto> ToAccessionWorklistDtoQueryable(this IQueryable<Accession> accession)
    {
        return accession?.Select(x => x == null 
            ? default 
            : new AccessionWorklistDto() 
            { 
                Id = x.Id, AccessionNumber = x.AccessionNumber, 
                Status = x.Status != null ? x.Status.Value : default,
                Patient = new AccessionWorklistDto.PatientDto() 
                { 
                    FirstName = x.Patient.FirstName, 
                    LastName = x.Patient.LastName, 
                    Age = x.Patient.Lifespan.Age,
                    Sex = x.Patient.Sex.Value
                },
                OrganizationName = x.HealthcareOrganization.Name,
                TestOrders = x.TestOrders.Select(x => new AccessionWorklistDto.TestOrderDto()
                {
                    TestName = x.Test.TestName, 
                    PanelName = x.PanelOrder.Panel.PanelName
                }).ToList() 
            });
    }
    
    public static EditableAccessionDto ToEditableAccessionDto(this Accession accession, IFileStorage fileStorage)
    {
        return new EditableAccessionDto()
        {
            Id = accession.Id,
            AccessionNumber = accession.AccessionNumber,
            Status = accession.Status != null ? accession.Status.Value : default,
            OrganizationId = accession?.HealthcareOrganization?.Id,
            Patient = accession?.Patient == null ? null : new EditableAccessionDto.PatientDto()
            {
                Id = accession.Patient.Id,
                FirstName = accession.Patient.FirstName,
                LastName = accession.Patient.LastName,
                Age = accession.Patient.Lifespan.Age,
                DateOfBirth = accession.Patient.Lifespan.DateOfBirth,
                Race = accession.Patient.Race.Value,
                Ethnicity = accession.Patient.Ethnicity.Value,
                Sex = accession.Patient.Sex.Value,
                InternalId = accession.Patient.InternalId
            },
            TestOrders = accession.TestOrders
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new EditableAccessionDto.TestOrderDto()
                {
                    Id = x.Id,
                    TestId = x.Test.Id,
                    TestName = x.Test.TestName,
                    Panel = new EditableAccessionDto.Panel()
                    {
                        Id = x.PanelOrder?.Id,
                        PanelName = x.PanelOrder?.Panel?.PanelName,
                        PanelCode = x.PanelOrder?.Panel?.PanelCode,
                        PanelOrderId = x.PanelOrder?.Id,
                        Type = x.PanelOrder?.Panel?.Type,
                        Version = x.PanelOrder?.Panel?.Version,
                    },
                    TestCode = x.Test.TestCode,
                    Status = x.Status != null ? x.Status.Value : default,
                    DueDate = x.DueDate,
                    TAT = x.TatSnapshot,
                    CancellationReason = x.CancellationReason != null ? x.CancellationReason.Value : default,
                    CancellationComments = x.CancellationComments,
                    IsPartOfPanel = x.IsPartOfPanel(),
                    Sample = new EditableAccessionDto.Sample()
                    {
                        Id = x.Sample?.Id,
                        SampleNumber = x?.Sample?.SampleNumber,
                    }
                })
                .OrderByDescending(x => x.TestName)
                .ToList() ?? new List<EditableAccessionDto.TestOrderDto>(),
            Attachments = accession.AccessionAttachments
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new EditableAccessionDto.AccessionAttachmentDto()
                {
                    Id = x.Id,
                    Type = x.Type,
                    Filename = x.Filename,
                    Comments = x.Comments,
                    DisplayName = x.DisplayName,
                    PreSignedUrl = x.GetPreSignedUrl(fileStorage)
                }).ToList() ?? new List<EditableAccessionDto.AccessionAttachmentDto>(),
            AccessionContacts = accession.AccessionContacts
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new EditableAccessionDto.AccessionContactDto()
                {
                    Id = x.Id,
                    TargetType = x.TargetType,
                    TargetValue = x.TargetValue,
                    FirstName = x.HealthcareOrganizationContact.FirstName,
                    LastName = x.HealthcareOrganizationContact.LastName,
                    Npi = x.HealthcareOrganizationContact.Npi,
                    OrganizationContactId = x.HealthcareOrganizationContact.Id
                }).ToList() ?? new List<EditableAccessionDto.AccessionContactDto>()
        };
    }
}