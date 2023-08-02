namespace PeakLims.Domain.Lifespans;

using Services;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public sealed class Lifespan : ValueObject
{
    // TODO age band
    // TODO phi friendly string (< 1 year, > 89 year) -- NOT MAPPED
    
    public int? KnownAge { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }

    public int? Age
    {
        get
        {
            if (KnownAge.HasValue)
                return KnownAge.Value;

            if (DateOfBirth.HasValue)
                return GetAgeInYears(DateOfBirth.Value);

            return null;
        }
    }

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

    public Lifespan(int? knownAge, DateOnly? dateOfBirth)
    {
        KnownAge = null;
        DateOfBirth = null;
            
        var hasDob = DateOnly.TryParse(dateOfBirth.ToString(), out var dob);
        var hasAge = int.TryParse(knownAge.ToString(), out var age);
            
        if(hasAge && !hasDob)
            CreateLifespanFromKnownAge(age);
        if(hasDob)
            CreateLifespanFromDateOfBirth(dob);
    }
    public Lifespan(int knownAge) => CreateLifespanFromKnownAge(knownAge);
    public Lifespan(DateOnly dob) => CreateLifespanFromDateOfBirth(dob);

    private void CreateLifespanFromKnownAge(int ageInYears)
    {
        if (ageInYears < 0)
            throw new ValidationException(nameof(Lifespan),"Age can not be less than zero years.");
        if (ageInYears > 120)
            throw new ValidationException(nameof(Lifespan),"Age can not be more than 120 years.");
            
        KnownAge = ageInYears;
        DateOfBirth = null;
    }
    
    private void CreateLifespanFromAge(int ageInYears)
    {
        if (ageInYears < 0)
            throw new ValidationException(nameof(Lifespan),"Age can not be less than zero years.");
        if (ageInYears > 120)
            throw new ValidationException(nameof(Lifespan),"Age can not be more than 120 years.");
        
        KnownAge = ageInYears;
        DateOfBirth = null;
    }

    private void CreateLifespanFromDateOfBirth(DateOnly dob)
    {
        if (dob.ToDateTime(TimeOnly.MinValue) > DateTime.UtcNow)
            throw new ValidationException(nameof(Lifespan), "Date of birth must be in the past");
        
        
        DateOfBirth = dob;
        KnownAge = null;
    }

    protected Lifespan() { } // EF Core
}