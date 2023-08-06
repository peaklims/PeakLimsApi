namespace PeakLims.UnitTests.Domain.Ethnicities;

using FluentAssertions;
using PeakLims.Domain.Ethnicities;
using Xunit;

public class EthnicityTests
{
    [Theory]
    [InlineData("gibberish")]
    [InlineData(null)]
    public void default_to_not_given(string input)
    {
        // Arrange
        // Act
        var race = Ethnicity.Of(input);

        // Assert
        race.Value.Should().Be(EthnicityEnum.NotGiven.Name);
    }
}