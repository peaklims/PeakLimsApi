namespace PeakLims.Domain.Tests.Dtos;

public sealed class TestForCreationDto
{
    public string TestCode { get; set; }
    public string TestName { get; set; }
    public string Methodology { get; set; }
    public string Platform { get; set; } 
    public int TurnAroundTime { get; set; }
}
