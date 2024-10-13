namespace PeakLims.UnitTests.Domain.Npis;

using PeakLims.Domain.Npis;

public class NpiTests
{
    [Theory]
    [InlineData("1154522944")]
    [InlineData("1104202696")]
    [InlineData("1780828673")]
    public void ValidNpi_ShouldCreateNpiObject(string npiValue)
    {
        // Act
        var npi = NPI.Of(npiValue);

        // Assert
        npi.Should().NotBeNull();
        npi.Value.Should().Be(npiValue);
    }

    [Theory]
    [InlineData("1098765432")] // Invalid check digit
    [InlineData("123456789")]   // Too short
    [InlineData("12345678901")] // Too long
    [InlineData("ABCDEFGHIJ")]  // Non-numeric
    [InlineData("115452294")]   // Too short, valid check digit but invalid length
    [InlineData("11545229440")] // Too long, valid check digit but invalid length
    public void InvalidNpi_ShouldThrowValidationException(string npiValue)
    {
        // Act
        var act = () => NPI.Of(npiValue);
        
        // Assert
        act.Should().Throw<FluentValidation.ValidationException>();
    }

    [Fact]
    public void NullOrWhitespaceNpi_ShouldSetValueToNull()
    {
        // Arrange
        var npiValue = "   ";

        // Act
        var npi = new NPI(npiValue);

        // Assert
        npi.Value.Should().BeNull();
    }
}
