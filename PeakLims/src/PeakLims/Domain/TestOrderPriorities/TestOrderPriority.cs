namespace PeakLims.Domain.TestOrderPriorities;

using Ardalis.SmartEnum;
using PeakLims.Exceptions;

public class TestOrderPriority : ValueObject
{
    private TestOrderPriorityEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!TestOrderPriorityEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public TestOrderPriority(string value)
    {
        Value = value;
    }
    public TestOrderPriority(TestOrderPriorityEnum value)
    {
        Value = value.Name;
    }

    public bool IsStat() => Value == Stat().Value;
    public static TestOrderPriority Of(string value) => new TestOrderPriority(value);
    public static implicit operator string(TestOrderPriority value) => value.Value;
    public static List<string> ListNames() => TestOrderPriorityEnum.List.Select(x => x.Name).ToList();

    public static TestOrderPriority Normal() => new TestOrderPriority(TestOrderPriorityEnum.Normal.Name);
    public static TestOrderPriority Stat() => new TestOrderPriority(TestOrderPriorityEnum.Stat.Name);

    protected TestOrderPriority() { } // EF Core
}

public abstract class TestOrderPriorityEnum : SmartEnum<TestOrderPriorityEnum>
{
    public static readonly TestOrderPriorityEnum Normal = new NormalType();
    public static readonly TestOrderPriorityEnum Stat = new StatType();

    protected TestOrderPriorityEnum(string name, int value) : base(name, value)
    {
    }

    private class NormalType() : TestOrderPriorityEnum("Normal", 0);

    private class StatType() : TestOrderPriorityEnum("STAT", 1);
}