namespace PeakLims.Domain.Percentages;

using SharedKernel.Domain;
using FluentValidation;

// source: https://github.com/asc-lab/better-code-with-ddd/blob/ef_core/LoanApplication.TacticalDdd/LoanApplication.TacticalDdd/DomainModel/Percent.cs
public class Percent : ValueObject
{
    public decimal Value { get; }
        
    public static readonly Percent Zero = new Percent(0M);

    public Percent(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Percent value cannot be negative");

        Value = value;
    }
        
    public static bool operator >(Percent one, Percent two) => one.CompareTo(two)>0;
        
    public static bool operator <(Percent one, Percent two) => one.CompareTo(two)<0;
        
    public static bool operator >=(Percent one, Percent two) => one.CompareTo(two)>=0;
        
    public static bool operator <=(Percent one, Percent two) => one.CompareTo(two)<=0;

    private int CompareTo(Percent other)
    {
        return Value.CompareTo(other.Value);
    }

    protected Percent() { } // EF Core
}

public static class PercentExtensions
{
    public static Percent Percent(this int value) => new Percent(value);
        
    public static Percent Percent(this decimal value) => new Percent(value);
}