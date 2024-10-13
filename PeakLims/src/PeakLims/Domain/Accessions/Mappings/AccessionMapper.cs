namespace PeakLims.Domain.Accessions.Mappings;

using Patients;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Services.External;
using Riok.Mapperly.Abstractions;
using TestOrders;

[Mapper]
public static partial class AccessionMapper
{
    public static partial AccessionDto ToAccessionDto(this Accession accession);
    public static partial IQueryable<AccessionDto> ToAccessionDtoQueryable(this IQueryable<Accession> accession);
    
    [MapProperty([nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age)], [nameof(AccessionWorklistDto.PatientDto.Age)])]
    private static partial AccessionWorklistDto.PatientDto ToAccessionWorklistDtoPatientDto(this Patient accessionAttachment);
    
    public static IQueryable<AccessionWorklistDto> ToAccessionWorklistDtoQueryable(this IQueryable<Accession> accession)
    {
        return accession?.Select(x => x == null 
            ? default 
            : new AccessionWorklistDto() 
            { 
                Id = x.Id, AccessionNumber = x.AccessionNumber, 
                Status = x.Status != null ? x.Status.Value : default,
                Patient = x.Patient.ToAccessionWorklistDtoPatientDto(),
                OrganizationName = x.HealthcareOrganization.Name,
                TestOrders = x.TestOrders.Select(x => new AccessionWorklistDto.TestOrderDto()
                {
                    TestName = x.Test.TestName, 
                    PanelName = x.PanelOrder.Panel.PanelName
                }).ToList() 
            });
    }
    
    [MapProperty([nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth)], [nameof(AccessionPageViewDto.PatientDto.DateOfBirth)
    ])]
    [MapProperty([nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age)], [nameof(AccessionPageViewDto.PatientDto.Age)
    ])]
    private static partial AccessionPageViewDto.PatientDto ToAccessionPageViewDtoPatientDto(this Patient accessionAttachment);
    
    public static AccessionPageViewDto ToEditableAccessionDto(this Accession accession, IFileStorage fileStorage)
    {
        var standaloneTestOrders = accession.TestOrders
            .Where(x => x.PanelOrder == null)
            .ToList();
        var panelOrders = accession.PanelOrders.ToList();
        var testOrdersCombined = standaloneTestOrders
            .Concat(panelOrders.SelectMany(x => x.TestOrders))
            .ToList();
        
        return new AccessionPageViewDto()
        {
            Id = accession.Id,
            AccessionNumber = accession.AccessionNumber,
            Status = accession.Status != null ? accession.Status.Value : default,
            OrganizationId = accession?.HealthcareOrganization?.Id,
            Patient = accession.Patient.ToAccessionPageViewDtoPatientDto(),
            TestOrders = testOrdersCombined
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new AccessionPageViewDto.TestOrderDto()
                {
                    Id = x.Id,
                    TestId = x.Test.Id,
                    TestName = x.Test.TestName,
                    Panel = new AccessionPageViewDto.Panel()
                    {
                        Id = x.PanelOrder?.Id,
                        PanelName = x.PanelOrder?.Panel?.PanelName,
                        PanelCode = x.PanelOrder?.Panel?.PanelCode,
                        PanelOrderId = x.PanelOrder?.Id,
                        Type = x.PanelOrder?.Panel?.Type,
                        Version = x.PanelOrder?.Panel?.Version,
                        Status = x?.PanelOrder?.Status()?.Value,
                    },
                    TestCode = x.Test.TestCode,
                    Status = x.Status != null ? x.Status.Value : default,
                    DueDate = x.DueDate,
                    TAT = x.TatSnapshot,
                    CancellationReason = x.CancellationReason != null ? x.CancellationReason.Value : default,
                    CancellationComments = x.CancellationComments,
                    IsPartOfPanel = x.IsPartOfPanel(),
                    Sample = new AccessionPageViewDto.Sample()
                    {
                        Id = x.Sample?.Id,
                        SampleNumber = x?.Sample?.SampleNumber,
                    }
                })
                .OrderByDescending(x => x.TestName)
                .ToList() ?? new List<AccessionPageViewDto.TestOrderDto>(),
            Attachments = accession.AccessionAttachments
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new AccessionPageViewDto.AccessionAttachmentDto()
                {
                    Id = x.Id,
                    Type = x.Type,
                    Filename = x.Filename,
                    Comments = x.Comments,
                    DisplayName = x.DisplayName,
                    PreSignedUrl = x.GetPreSignedUrl(fileStorage)
                }).ToList() ?? new List<AccessionPageViewDto.AccessionAttachmentDto>(),
            AccessionContacts = accession.AccessionContacts
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new AccessionPageViewDto.AccessionContactDto()
                {
                    Id = x.Id,
                    TargetType = x.TargetType,
                    TargetValue = x.TargetValue,
                    FirstName = x.HealthcareOrganizationContact.FirstName,
                    LastName = x.HealthcareOrganizationContact.LastName,
                    Npi = x.HealthcareOrganizationContact.Npi,
                    OrganizationContactId = x.HealthcareOrganizationContact.Id
                }).ToList() ?? new List<AccessionPageViewDto.AccessionContactDto>()
        };
    }
}