namespace PeakLims.Domain.Accessions.Mappings;

using PeakLims.Domain.Accessions.Dtos;
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
                    Age = x.Patient.Lifespan.Age 
                },
                TestOrders = x.TestOrders.Select(x => new AccessionWorklistDto.TestOrderDto()
                {
                    TestName = x.Test.TestName, 
                    PanelName = x.AssociatedPanel.PanelName
                }).ToList() 
            });
    }
    
    public static EditableAccessionDto ToEditableAccessionDto(this Accession accession)
    {
        return new EditableAccessionDto()
        {
            Id = accession.Id,
            AccessionNumber = accession.AccessionNumber,
            Status = accession.Status != null ? accession.Status.Value : default,
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
            }
        };
    }
}