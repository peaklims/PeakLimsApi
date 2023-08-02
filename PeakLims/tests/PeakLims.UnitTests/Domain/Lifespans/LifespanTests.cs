namespace PeakLims.UnitTests.Domain.Lifespans;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Lifespans;
using Xunit;

public class LifespanTests
{
    private readonly Faker _faker;

    public LifespanTests()
    {
        _faker = new Faker();
    }
    
    [Theory]
    [InlineData(-18, 0)]
    [InlineData(-2250, 6)]
    [InlineData(-365, 1)]
    [InlineData(-364, 0)]
    [InlineData(0, 0)]
    public void can_get_lifespan_from_dateonly(int daysOld, int age)
    {
        var dob = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(daysOld);
        var lifespan = new Lifespan(dob);

        lifespan.DateOfBirth.Should().Be(dob);
        lifespan.KnownAge.Should().BeNull();
        lifespan.Age.Should().Be(age);
        lifespan.GetAgeInDays().Should().Be(daysOld*-1);
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(6, 6)]
    [InlineData(1, 1)]
    public void can_get_lifespan_from_age(int inputAge, int expectedAge)
    {
        var lifespan = new Lifespan(inputAge);

        lifespan.KnownAge.Should().Be(expectedAge);
        lifespan.DateOfBirth.Should().Be(null);
        lifespan.GetAgeInDays().Should().Be(null);
    }
    
    [Fact]
    public void dob_trumps_age()
    {
        var validYearsOld = _faker.Random.Int(min: 0, max: 120);
        var invalidYearsOld = _faker.Random.Int(min: 0, max: 120);
        var dob = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-validYearsOld);
        var lifespan = new Lifespan(invalidYearsOld, dob);

        lifespan.DateOfBirth.Should().Be(dob);
        lifespan.Age.Should().Be(validYearsOld);
        lifespan.KnownAge.Should().BeNull();
    }
    
    [Fact]
    public void can_not_be_less_than_0_years_using_dateonly()
    {
        var dob = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        var lifespan = () => new Lifespan(dob);
        lifespan.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }
    
    [Fact]
    public void can_not_be_more_than_120_years_using_dateonly()
    {
        var dob = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(120);
        var lifespan = () => new Lifespan(dob);
        lifespan.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }
    
    [Fact]
    public void can_not_be_less_than_0_years_using_age()
    {
        var lifespan = () => new Lifespan(-1);
        lifespan.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }
    
    [Fact]
    public void can_not_be_more_than_120_years_using_age()
    {
        var lifespan = () => new Lifespan(121);
        lifespan.Should().Throw<SharedKernel.Exceptions.ValidationException>();
    }
}