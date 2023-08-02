namespace PeakLims.Domain.Races;

using Ardalis.SmartEnum;
using SharedKernel.Domain;

public class Race : ValueObject
{
    private RaceEnum _race;
    public string Value
    {
        get => _race.Name;
        private set
        {
            if (!RaceEnum.TryFromName(value, true, out var parsed))
                parsed = RaceEnum.Unknown;

            _race = parsed;
        }
    }
    
    public Race(string value) => Value = value;
    public Race(RaceEnum value) => Value = value.Name;

    public static Race Of(string value) => new Race(value);
    public static implicit operator string(Race value) => value.Value;
    public static List<string> ListNames() => RaceEnum.List.Select(x => x.Name).ToList();

    public static Race Unknown() => new Race(RaceEnum.Unknown.Name);
    public static Race AmericanIndianAlaskaNative() => new Race(RaceEnum.AmericanIndianAlaskaNative.Name);
    public static Race BlackOrAfricanAmerican() => new Race(RaceEnum.BlackOrAfricanAmerican.Name);
    public static Race NativeHawaiianPacificIslander() => new Race(RaceEnum.NativeHawaiianPacificIslander.Name);
    public static Race White() => new Race(RaceEnum.White.Name);
    public static Race Asian() => new Race(RaceEnum.Asian.Name);

    protected Race() { } // EF Core
}

public abstract class RaceEnum : SmartEnum<RaceEnum>
{
    public static readonly RaceEnum Unknown = new UnknownType();
    public static readonly RaceEnum AmericanIndianAlaskaNative = new AmericanIndianAlaskaNativeType();
    public static readonly RaceEnum BlackOrAfricanAmerican = new BlackOrAfricanAmericanType();
    public static readonly RaceEnum NativeHawaiianPacificIslander = new NativeHawaiianPacificIslanderType();
    public static readonly RaceEnum White = new WhiteType();
    public static readonly RaceEnum Asian = new AsianType();

    protected RaceEnum(string name, int value) : base(name, value)
    {
    }

    private class UnknownType : RaceEnum
    {
        public UnknownType() : base("Unknown", 0)
        {
        }
    }

    private class AmericanIndianAlaskaNativeType : RaceEnum
    {
        public AmericanIndianAlaskaNativeType() : base("American Indian/Alaska Native", 1)
        {
        }
    }

    private class AsianType : RaceEnum
    {
        public AsianType() : base("Asian", 2)
        {
        }
    }

    private class BlackOrAfricanAmericanType : RaceEnum
    {
        public BlackOrAfricanAmericanType() : base("Black or African American", 3)
        {
        }
    }

    private class NativeHawaiianPacificIslanderType : RaceEnum
    {
        public NativeHawaiianPacificIslanderType() : base("Native Hawaiian/Pacific Islander", 4)
        {
        }
    }

    private class WhiteType : RaceEnum
    {
        public WhiteType() : base("White", 5)
        {
        }
    }
}