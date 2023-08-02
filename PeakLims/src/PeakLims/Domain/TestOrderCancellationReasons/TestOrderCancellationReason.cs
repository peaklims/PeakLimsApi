namespace PeakLims.Domain.TestOrderCancellationReasons;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class TestOrderCancellationReason : ValueObject
{
    private TestOrderCancellationReasonEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!TestOrderCancellationReasonEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public TestOrderCancellationReason(string value)
    {
        Value = value;
    }
    public TestOrderCancellationReason(TestOrderCancellationReasonEnum value)
    {
        Value = value.Name;
    }
    
    public static TestOrderCancellationReason Of(string value) => new TestOrderCancellationReason(value);
    public static implicit operator string(TestOrderCancellationReason value) => value.Value;
    public static List<string> ListNames() => TestOrderCancellationReasonEnum.List.Select(x => x.Name).ToList();

    public static TestOrderCancellationReason Qns() => new TestOrderCancellationReason(TestOrderCancellationReasonEnum.Qns.Name);
    public static TestOrderCancellationReason Abandoned() => new TestOrderCancellationReason(TestOrderCancellationReasonEnum.Abandoned.Name);
    public static TestOrderCancellationReason Other() => new TestOrderCancellationReason(TestOrderCancellationReasonEnum.Other.Name);

    protected TestOrderCancellationReason() { } // EF Core
}

public abstract class TestOrderCancellationReasonEnum : SmartEnum<TestOrderCancellationReasonEnum>
{
    public static readonly TestOrderCancellationReasonEnum Qns = new QnsType();
    public static readonly TestOrderCancellationReasonEnum Abandoned = new AbandonedType();
    public static readonly TestOrderCancellationReasonEnum Other = new OtherType();

    protected TestOrderCancellationReasonEnum(string name, int value) : base(name, value)
    {
    }

    private class QnsType : TestOrderCancellationReasonEnum
    {
        public QnsType() : base("QNS", 0)
        {
        }
    }

    private class AbandonedType : TestOrderCancellationReasonEnum
    {
        public AbandonedType() : base("Abandoned", 1)
        {
        }
    }

    private class OtherType : TestOrderCancellationReasonEnum
    {
        public OtherType() : base("Other", 100)
        {
        }
    }
}