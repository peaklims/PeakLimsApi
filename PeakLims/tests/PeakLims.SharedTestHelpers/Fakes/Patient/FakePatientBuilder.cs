namespace PeakLims.SharedTestHelpers.Fakes.Patient;

using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Models;

public class FakePatientBuilder
{
    private PatientForCreation _creationData = new FakePatientForCreation().Generate();

    public FakePatientBuilder WithModel(PatientForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakePatientBuilder WithFirstName(string firstName)
    {
        _creationData.FirstName = firstName;
        return this;
    }
    
    public FakePatientBuilder WithLastName(string lastName)
    {
        _creationData.LastName = lastName;
        return this;
    }
    
    public FakePatientBuilder WithDateOfBirth(DateOnly? dateOfBirth)
    {
        _creationData.DateOfBirth = dateOfBirth;
        return this;
    }
    
    public FakePatientBuilder WithAge(int? age)
    {
        _creationData.Age = age;
        return this;
    }
    
    public FakePatientBuilder WithSex(string sex)
    {
        _creationData.Sex = sex;
        return this;
    }
    
    public FakePatientBuilder WithRace(string race)
    {
        _creationData.Race = race;
        return this;
    }
    
    public FakePatientBuilder WithEthnicity(string ethnicity)
    {
        _creationData.Ethnicity = ethnicity;
        return this;
    }
    
    public Patient Build()
    {
        var result = Patient.Create(_creationData);
        return result;
    }
}