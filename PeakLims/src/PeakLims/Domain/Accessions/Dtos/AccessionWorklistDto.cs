namespace PeakLims.Domain.Accessions.Dtos;

public sealed class AccessionWorklistDto
{
    public Guid Id { get; set; }
    public string AccessionNumber { get; set; }
    public string Status { get; set; }
    public PatientDto Patient { get; set; }
    public List<TestOrderDto> TestOrders { get; set; } = new();

    
    public sealed class PatientDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
    }

    public sealed class TestOrderDto
    {
        public string TestName { get; set; }
        public string PanelName { get; set; }
    }
}
