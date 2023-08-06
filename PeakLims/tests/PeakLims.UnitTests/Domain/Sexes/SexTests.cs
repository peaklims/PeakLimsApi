namespace PeakLims.UnitTests.Domain.Sexes;

using FluentAssertions;
using PeakLims.Domain.Sexes;
using Xunit;

public class SexTests
{
    [Theory]
    [InlineData("gibberish")]
    [InlineData(null)]
    public void default_to_not_given(string input)
    {
        // Arrange
        // Act
        var sex = Sex.Of(input);

        // Assert
        sex.Value.Should().Be(SexEnum.NotGiven.Name);
    }
    
    [Theory]
    [InlineData("m")]
    [InlineData("M")]
    [InlineData("Male")]
    [InlineData("MALE")]
    [InlineData("male")]
    public void can_transform_for_male(string input)
    {
        // Arrange
        // Act
        var sex = Sex.Of(input);

        // Assert
        sex.Value.Should().Be(SexEnum.Male.Name);
    }
    
    [Theory]
    [InlineData("f")]
    [InlineData("F")]
    [InlineData("Female")]
    [InlineData("FEMALE")]
    [InlineData("female")]
    public void can_transform_for_female(string input)
    {
        // Arrange
        // Act
        var sex = Sex.Of(input);

        // Assert
        sex.Value.Should().Be(SexEnum.Female.Name);
    }
}