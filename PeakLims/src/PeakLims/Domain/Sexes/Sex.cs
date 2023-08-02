namespace PeakLims.Domain.Sexes;

using Ardalis.SmartEnum;
using SharedKernel.Domain;

public class Sex : ValueObject
{
    private SexEnum _sex;
    public string Value
    {
        get => _sex.Name;
        private set
        {
            if(string.IsNullOrEmpty(value))
                value = SexEnum.Unknown.Name;
            if (value.Trim().Equals("m", StringComparison.InvariantCultureIgnoreCase))
                value = SexEnum.Male.Name;
            if (value.Trim().Equals("f", StringComparison.InvariantCultureIgnoreCase))
                value = SexEnum.Female.Name;

            if (!SexEnum.TryFromName(value, true, out var parsed))
                parsed = SexEnum.Unknown;

            _sex = parsed;
        }
    }
    
    public Sex(string value)
    {
        Value = value;
    }
    public Sex(SexEnum value)
    {
        Value = value.Name;
    }
    
    public static Sex Of(string value) => new Sex(value);
    public static implicit operator string(Sex value) => value.Value;
    public static List<string> ListNames() => SexEnum.List.Select(x => x.Name).ToList();

    public static Sex Unknown() => new Sex(SexEnum.Unknown.Name);
    public static Sex Male() => new Sex(SexEnum.Male.Name);
    public static Sex Female() => new Sex(SexEnum.Female.Name);

    protected Sex() { } // EF Core
}

public abstract class SexEnum : SmartEnum<SexEnum>
{
    public static readonly SexEnum Unknown = new UnknownType();
    public static readonly SexEnum Male = new MaleType();
    public static readonly SexEnum Female = new FemaleType();

    protected SexEnum(string name, int value) : base(name, value)
    {
    }

    private class UnknownType : SexEnum
    {
        public UnknownType() : base("Unknown", 0)
        {
        }
    }

    private class MaleType : SexEnum
    {
        public MaleType() : base("Male", 1)
        {
        }
    }

    private class FemaleType : SexEnum
    {
        public FemaleType() : base("Female", 2)
        {
        }
    }
}