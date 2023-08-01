namespace PeakLims.Domain.Tests.Dtos;

public sealed class TestDto
{
    public Guid Id { get; set; }
    public string TestCode { get; set; }
    public string TestName { get; set; }
    public string Methodology { get; set; }
    public string Platform { get; set; }
    public int Version { get; set; }
    public int TurnAroundTime { get; set; }
    public string Status { get; set; }
}
