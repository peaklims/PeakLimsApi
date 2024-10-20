namespace PeakLims.Domain.Gaia.Models;

using Bogus.DataSets;

public class PersonInfo
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Name.Gender Sex { get; set; }
}