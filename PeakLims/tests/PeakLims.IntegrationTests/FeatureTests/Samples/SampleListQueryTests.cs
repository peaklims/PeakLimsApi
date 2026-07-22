namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.Domain.Samples.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.Samples.Features;
using FluentAssertions;
using Domain;

using System.Threading.Tasks;
using Exceptions;

public class SampleListQueryTests : TestBase
{

    [Fact]
    public async Task can_get_sample_list()
    {
        // Arrange
        // Samples are filtered by the owning patient's organization (Patient.OrganizationId), so they must
        // belong to a patient in the org to be visible. Attaching them to a patient here keeps the test
        // correct when it runs in parallel with others.
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientBuilder().Build();
        patient.AddSample(new FakeSampleBuilder().Build());
        patient.AddSample(new FakeSampleBuilder().Build());
        var queryParameters = new SampleParametersDto();

        await testingServiceScope.InsertAsync(patient);

        // Act
        var query = new GetSampleList.Query(queryParameters);
        var samples = await testingServiceScope.SendAsync(query);

        // Assert
        samples.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadSamples);
        var queryParameters = new SampleParametersDto();

        // Act
        var command = new GetSampleList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}