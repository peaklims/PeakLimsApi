namespace PeakLims.Domain.Lifespans.Dtos;
            
public class LifespanDto
{
    public int? Age { get; set; }
    public int? AgeInDays { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}