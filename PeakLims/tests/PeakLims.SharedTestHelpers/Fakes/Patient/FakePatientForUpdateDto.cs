namespace PeakLims.SharedTestHelpers.Fakes.Patient;

using AutoBogus;
using Domain.Ethnicities;
using Domain.Races;
using Domain.Sexes;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Dtos;

public sealed class FakePatientForUpdateDto : AutoFaker<PatientForUpdateDto>
{
    public FakePatientForUpdateDto()
    {
        RuleFor(x => x.DateOfBirth, f=> f.Date.PastDateOnly());
        RuleFor(x => x.Age, _ => null);
        RuleFor(x => x.Sex, f => f.PickRandom(Sex.ListNames()));
        RuleFor(x => x.Race, f => f.PickRandom(Race.ListNames()));
        RuleFor(x => x.Ethnicity, f => f.PickRandom(Ethnicity.ListNames()));
    }
}