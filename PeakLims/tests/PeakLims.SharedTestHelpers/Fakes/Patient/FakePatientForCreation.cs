namespace PeakLims.SharedTestHelpers.Fakes.Patient;

using AutoBogus;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Models;

public sealed class FakePatientForCreation : AutoFaker<PatientForCreation>
{
    public FakePatientForCreation()
    {
    }
}