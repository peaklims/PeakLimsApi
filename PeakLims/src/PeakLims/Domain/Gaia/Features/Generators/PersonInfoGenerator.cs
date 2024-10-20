namespace PeakLims.Domain.Gaia.Features.Generators;

using Bogus.DataSets;
using Models;
using Serilog;
using Soenneker.Utils.AutoBogus;

public static class PersonInfoGenerator
{
    public static List<PersonInfo> Generate(int? count = null)
    {
        var autoFaker = new AutoFaker<PersonInfo>();
        autoFaker.RuleFor(u => u.Sex, f => f.PickRandom<Name.Gender>());
        autoFaker.RuleFor(x => x.FirstName, (f, u) => f.Name.FirstName(u.Sex));
        autoFaker.RuleFor(x => x.LastName, (f, u) => f.Name.LastName(u.Sex));
        
        var random = new Random();
        var numPeople = count ?? random.Next(30, 75);
        var names = autoFaker.Generate(numPeople);
        return names;
    }
}