namespace PeakLims.Domain.TestStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class TestStatus : ValueObject
{
    private TestStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!TestStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public TestStatus(string value)
    {
        Value = value;
    }
    public TestStatus(TestStatusEnum value)
    {
        Value = value.Name;
    }

    public bool IsActive() => Value == Active().Value;
    public static TestStatus Of(string value) => new TestStatus(value);
    public static implicit operator string(TestStatus value) => value.Value;
    public static List<string> ListNames() => TestStatusEnum.List.Select(x => x.Name).ToList();

    public static TestStatus Draft() => new TestStatus(TestStatusEnum.Draft.Name);
    public static TestStatus Active() => new TestStatus(TestStatusEnum.Active.Name);
    public static TestStatus Inactive() => new TestStatus(TestStatusEnum.Inactive.Name);

    protected TestStatus() { } // EF Core
}

public abstract class TestStatusEnum : SmartEnum<TestStatusEnum>
{
    public static readonly TestStatusEnum Draft = new DraftType();
    public static readonly TestStatusEnum Active = new ActiveType();
    public static readonly TestStatusEnum Inactive = new InactiveType();

    protected TestStatusEnum(string name, int value) : base(name, value)
    {
    }

    private class DraftType : TestStatusEnum
    {
        public DraftType() : base("Draft", 0)
        {
        }
    }

    private class ActiveType : TestStatusEnum
    {
        public ActiveType() : base("Active", 1)
        {
        }
    }

    private class InactiveType : TestStatusEnum
    {
        public InactiveType() : base("Inactive", 2)
        {
        }
    }
}