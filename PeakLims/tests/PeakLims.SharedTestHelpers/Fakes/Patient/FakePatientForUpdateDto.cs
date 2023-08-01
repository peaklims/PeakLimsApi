namespace PeakLims.SharedTestHelpers.Fakes.Patient;

using AutoBogus;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Dtos;

public sealed class FakePatientForUpdateDto : AutoFaker<PatientForUpdateDto>
{
    public FakePatientForUpdateDto()
    {
    }
}