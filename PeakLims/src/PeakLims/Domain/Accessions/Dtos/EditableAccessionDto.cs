namespace PeakLims.Domain.Accessions.Dtos;

public sealed record EditableAccessionDto
{
    public Guid Id { get; set; }
    public string AccessionNumber { get; set; }
    public string Status { get; set; }
    public PatientDto Patient { get; set; } = null!;
    public Guid? OrganizationId { get; set; }
    public List<AccessionContactDto> AccessionContacts { get; set; } = new();
    public List<TestOrderDto> TestOrders { get; set; }

    public sealed record PatientDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int? Age { get; set; }
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

    public class TestOrderDto
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public string TestName { get; set; }
        public Guid? PanelId { get; set; }
        public string PanelName { get; set; }
        public string Status { get; set; }
        public DateOnly? DueDate { get; set; }
        public int? TAT { get; set; }
        public string CancellationReason { get; set; }
        public string CancellationComments { get; set; }
        public bool IsPartOfPanel { get; set; }
    }
}
