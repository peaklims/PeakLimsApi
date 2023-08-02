namespace PeakLims.Domain.SampleTypes;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class SampleType : ValueObject
{
    private SampleTypeEnum _sampleType;
    public string Value
    {
        get => _sampleType.Name;
        private set
        {
            if (!SampleTypeEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _sampleType = parsed;
        }
    }
    
    public SampleType(string value) => Value = value;
    public SampleType(SampleTypeEnum value) => Value = value.Name;

    public static SampleType Of(string value) => new SampleType(value);
    public static implicit operator string(SampleType value) => value.Value;
    public static List<string> ListNames() => SampleTypeEnum.List.Select(x => x.Name).ToList();

    public static SampleType Blood() => new SampleType(SampleTypeEnum.Blood.Name);
    public static SampleType Sputum() => new SampleType(SampleTypeEnum.Sputum.Name);
    public static SampleType Data() => new SampleType(SampleTypeEnum.Data.Name);

    protected SampleType() { } // EF Core
}

public abstract class SampleTypeEnum : SmartEnum<SampleTypeEnum>
{
    public static readonly SampleTypeEnum Blood = new BloodType();
    public static readonly SampleTypeEnum Sputum = new SputumType();
    public static readonly SampleTypeEnum Data = new DataType();

    protected SampleTypeEnum(string name, int value) : base(name, value)
    {
    }

    private class BloodType : SampleTypeEnum
    {
        public BloodType() : base("Blood", 0)
        {
        }
    }

    private class SputumType : SampleTypeEnum
    {
        public SputumType() : base("Sputum", 1)
        {
        }
    }

    private class DataType : SampleTypeEnum
    {
        public DataType() : base("Data", 100)
        {
        }
    }
}