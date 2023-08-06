namespace PeakLims.UnitTests.Domain.Races;

using FluentAssertions;
using PeakLims.Domain.Races;
using Xunit;

public class RaceTests
{
    [Theory]
    [InlineData("gibberish")]
    [InlineData(null)]
    public void default_to_not_given(string input)
    {
        // Arrange
        // Act
        var race = Race.Of(input);

        // Assert
        race.Value.Should().Be(RaceEnum.NotGiven.Name);
    }
}