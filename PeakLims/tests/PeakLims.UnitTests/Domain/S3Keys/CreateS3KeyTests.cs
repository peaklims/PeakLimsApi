namespace PeakLims.UnitTests.Domain.S3Keys;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.S3Keys;
using Xunit;

public class CreateS3KeyTests
{
    private readonly Faker _faker;

    public CreateS3KeyTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_key()
    {
        var keyValue = _faker.System.FileName();
        var keyVo = new S3Key(keyValue);
        keyVo.Value.Should().Be(keyValue);
    }

    [Fact]
    public void can_not_add_invalid_key()
    {
        var validS3Key = _faker.Lorem.Word();
        var act = () => new S3Key(validS3Key);
        act.Should().Throw<FluentValidation.ValidationException>();
    }

    [Fact]
    public void key_can_be_null()
    {
        var keyVo = new S3Key(null);
        keyVo.Value.Should().BeNull();
    }
}