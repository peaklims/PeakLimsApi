namespace PeakLims.Domain.Lifespans;

using Services;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public sealed class Lifespan : ValueObject
{
    // TODO age band
    // TODO phi friendly string (< 1 year, > 89 year) -- NOT MAPPED
    
    public int? Age { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }

    public int? GetAgeInDays()
    {
        if (DateOfBirth.HasValue)
            return GetAgeInDays(DateOfBirth.Value);

        return null;
    }

    private static int GetAgeInDays(DateOnly dob)
    {
        var dateSpan = DateTime.UtcNow - dob.ToDateTime(TimeOnly.MinValue);
        return dateSpan.Days;
    }
    
    private static int GetAgeInYears(DateOnly dob)
    {
        var ageInYears = DateTime.UtcNow.Year - dob.ToDateTime(TimeOnly.MinValue).Year;
        if (dob.ToDateTime(TimeOnly.MinValue).AddYears(ageInYears) > DateTime.UtcNow)
            ageInYears--;

        return ageInYears;
    }

    public Lifespan(int? exactAge, DateOnly? dateOfBirth)
    {
        DateOfBirth = null;
        Age = null;
        
        var hasDob = DateOnly.TryParse(dateOfBirth.ToString(), out var dob);
        var hasAge = int.TryParse(exactAge.ToString(), out var age);
        
        if(hasAge && !hasDob)
            CreateLifespanFromAge(age);
        if(hasDob)
            CreateLifespanFromDateOfBirth(dob);
    }
    
    public Lifespan(int exactAge) => CreateLifespanFromAge(exactAge);
    public Lifespan(DateOnly dob) => CreateLifespanFromDateOfBirth(dob);

    private void CreateLifespanFromAge(int ageInYears)
    {
        if (ageInYears < 0)
            throw new ValidationException(nameof(Lifespan),"Age can not be less than zero years.");
        if (ageInYears > 120)
            throw new ValidationException(nameof(Lifespan),"Age can not be more than 120 years.");
        
        Age = ageInYears;
        DateOfBirth = null;
    }

    private void CreateLifespanFromDateOfBirth(DateOnly dob)
    {
        if (dob.ToDateTime(TimeOnly.MinValue) > DateTime.UtcNow)
            throw new ValidationException(nameof(Lifespan), "Date of birth must be in the past");
        
        
        DateOfBirth = dob;
        Age = GetAgeInYears(dob);
    }

    protected Lifespan() { } // EF Core
}