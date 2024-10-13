namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using Domain.AccessionContacts;
using Domain.AccessionContacts;
using Domain.HealthcareOrganizations;
using Domain.Panels;
using Domain.Patients;
using Domain.Tests;
using HealthcareOrganization;
using AccessionContact;
using Panel;
using Patient;
using PeakLims.Domain.Accessions;
using Sample;
using Test;
using Utilities;

public class FakeAccessionBuilder
{
    private readonly List<Test> _tests = [];
    private Patient _patient = new FakePatientBuilder().Build();
    private HealthcareOrganization _healthcareOrganization = null;
    private AccessionContact _accessionContact = null;
    private Guid _organizationId = TestingConsts.DefaultTestingOrganizationId;
    
    public FakeAccessionBuilder WithTest(Test test)
    {
        _tests.Add(test);
        return this;
    }
    
    public FakeAccessionBuilder WithPatient(Patient patient)
    {
        _patient = patient;
        return this;
    }
    
    public FakeAccessionBuilder WithoutPatient()
    {
        _patient = null;
        return this;
    }
    
    public FakeAccessionBuilder WithOrganizationId(Guid organizationId)
    {
        _organizationId = organizationId;
        return this;
    }

    public FakeAccessionBuilder WithRandomTest()
    {
        var test = new FakeTestBuilder().Build().Activate();
        _tests.Add(test);
        return this;
    }

    public FakeAccessionBuilder WithSetupForValidReadyForTestingTransition()
    {
        if(_patient == null)
            _patient = new FakePatientBuilder().Build();
        
        if(_healthcareOrganization == null)
            _healthcareOrganization = new FakeHealthcareOrganizationBuilder().Build().Activate();
        
        if(_accessionContact == null)
            _accessionContact = new FakeAccessionContactBuilder().Build();

        if (_tests.Count == 0)
            WithRandomTest();

        return this;
    }

    public Accession Build()
    {
        var result = Accession.Create(_organizationId);
        foreach (var test in _tests)
        {
            result.AddTest(test);
            var sample = new FakeSampleBuilder().Build();
            result.TestOrders
                .FirstOrDefault(x => x.Test.TestCode == test.TestCode)
                !.SetSample(sample);
        }
        if (_patient != null)
        {
            result.SetPatient(_patient);
        }
        if (_healthcareOrganization != null)
        {
            result.SetHealthcareOrganization(_healthcareOrganization);
        }
        if (_accessionContact != null)
        {
            result.AddContact(_accessionContact);
        }
        
        return result;
    }
}