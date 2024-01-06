namespace PeakLims.Domain.Accessions.Dtos;

using Destructurama.Attributed;
using TestOrderStatuses;

public sealed record EditableAccessionDto
{
    public Guid Id { get; set; }
    public string AccessionNumber { get; set; }
    public string Status { get; set; }
    public PatientDto Patient { get; set; } = null!;
    public Guid? OrganizationId { get; set; }
    public List<AccessionContactDto> AccessionContacts { get; set; } = new();
    public List<TestOrderDto> TestOrders { get; set; }
    public List<AccessionAttachmentDto> Attachments { get; set; } = new();

    public sealed record PatientDto
    {
        public Guid Id { get; set; }
    
        [LogMasked]
        public string FirstName { get; set; }
    
        [LogMasked]
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int? Age { get; set; }
    
        [LogMasked]
        public DateOnly? DateOfBirth { get; set; }
        public string Race { get; set; }
        public string Ethnicity { get; set; }
        public string InternalId { get; set; }
    }
    
    public sealed record AccessionContactDto
    {
        public Guid Id { get; set; }
        public string TargetType { get; set; }
        public string TargetValue { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Npi { get; set; } = null!;
        public Guid OrganizationContactId { get; set; }
    }

    public sealed record TestOrderDto
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public string TestName { get; set; }
        public string TestCode { get; set; }
        public Panel Panel { get; set; }
        public string Status { get; set; }
        public DateOnly? DueDate { get; set; }
        public int? TAT { get; set; }
        public string CancellationReason { get; set; }
        public string CancellationComments { get; set; }
        public bool IsPartOfPanel { get; set; }
        public Sample Sample { get; set; } = new Sample();
    }

    public sealed record Sample
    {
        public Guid? Id { get; set; } = default!;
        public string? SampleNumber { get; set; } = default!;
    }
    
    public sealed record Panel
    {
        public Guid? Id { get; set; }
        public string PanelCode { get; set; }
        public string PanelName { get; set; }
        public string Type { get; set; }
        public int? Version { get; set; }
        public Guid? PanelOrderId { get; set; }
    }
    
    public sealed record AccessionAttachmentDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
    
        [LogMasked]
        public string Filename { get; set; }
        public string Comments { get; set; }
        public string PreSignedUrl { get; set; }
        public string DisplayName { get; set; }
    }
}
