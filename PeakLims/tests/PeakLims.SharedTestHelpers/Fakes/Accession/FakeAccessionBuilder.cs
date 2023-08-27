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

public class FakeAccessionBuilder
{
    private List<Panel> _panels = new List<Panel>();
    private List<Test> _tests = new List<Test>();
    private Patient _patient = null;
    private HealthcareOrganization _healthcareOrganization = null;
    private AccessionContact _accessionContact = null;

    public FakeAccessionBuilder WithPanel(Panel panel)
    {
        _panels.Add(panel);
        return this;
    }
    
    public FakeAccessionBuilder WithTest(Test test)
    {
        _tests.Add(test);
        return this;
    }

    public FakeAccessionBuilder WithRandomTest()
    {
        var test = new FakeTestBuilder().Build().Activate();
        _tests.Add(test);
        return this;
    }
    
    public FakeAccessionBuilder WithRandomPanel()
    {
        var panel = new FakePanelBuilder().Build().Activate();
        _panels.Add(panel);
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
        var result = Accession.Create();
        foreach (var panel in _panels)
        {
            result.AddPanel(panel);
        }
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