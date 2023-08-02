namespace PeakLims.Domain.Ethnicities;

using Ardalis.SmartEnum;
using SharedKernel.Domain;

public class Ethnicity : ValueObject
{
    private EthnicityEnum _race;
    public string Value
    {
        get => _race.Name;
        private set
        {
            if (!EthnicityEnum.TryFromName(value, true, out var parsed))
                parsed = EthnicityEnum.Unknown;

            _race = parsed;
        }
    }
    
    public Ethnicity(string value) => Value = value;
    public Ethnicity(EthnicityEnum value) => Value = value.Name;

    public static Ethnicity Of(string value) => new Ethnicity(value);
    public static implicit operator string(Ethnicity value) => value.Value;
    public static List<string> ListNames() => EthnicityEnum.List.Select(x => x.Name).ToList();

    public static Ethnicity Unknown() => new Ethnicity(EthnicityEnum.Unknown.Name);
    public static Ethnicity HispanicLatino() => new Ethnicity(EthnicityEnum.HispanicLatino.Name);
    public static Ethnicity NonHispanicLatino() => new Ethnicity(EthnicityEnum.NonHispanicLatino.Name);

    protected Ethnicity() { } // EF Core
}

public abstract class EthnicityEnum : SmartEnum<EthnicityEnum>
{
    public static readonly EthnicityEnum Unknown = new UnknownType();
    public static readonly EthnicityEnum HispanicLatino = new HispanicLatinoType();
    public static readonly EthnicityEnum NonHispanicLatino = new NonHispanicLatinoType();

    protected EthnicityEnum(string name, int value) : base(name, value)
    {
    }

    private class UnknownType : EthnicityEnum
    {
        public UnknownType() : base("Unknown", 0)
        {
        }
    }

    private class HispanicLatinoType : EthnicityEnum
    {
        public HispanicLatinoType() : base("Hispanic/Latino", 1)
        {
        }
    }

    private class NonHispanicLatinoType : EthnicityEnum
    {
        public NonHispanicLatinoType() : base("Not Hispanic/Latino", 2)
        {
        }
    }
}