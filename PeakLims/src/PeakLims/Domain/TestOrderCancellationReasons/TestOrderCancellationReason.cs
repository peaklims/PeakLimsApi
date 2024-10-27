namespace PeakLims.Domain.TestOrderCancellationReasons;

using Ardalis.SmartEnum;
using Exceptions;

public class TestOrderCancellationReason : ValueObject
{
    private TestOrderCancellationReasonEnum _reason;
    public string Value
    {
        get => _reason.Name;
        private set
        {
            if (value == null)
                _reason = null;
            
            if (!TestOrderCancellationReasonEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException($"Invalid Cancellation Reason. Please use one of the following: {string.Join(", ", ListNames())}");

            _reason = parsed;
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
    public static implicit operator string(TestOrderCancellationReason value) => value == null ? null : value.Value;
    public static List<string> ListNames() => TestOrderCancellationReasonEnum.List.Select(x => x.Name).ToList();

    public static TestOrderCancellationReason Qns() => new TestOrderCancellationReason(TestOrderCancellationReasonEnum.Qns.Name);
    public static TestOrderCancellationReason Abandoned() => new TestOrderCancellationReason(TestOrderCancellationReasonEnum.Abandoned.Name);
    public static TestOrderCancellationReason Other() => new TestOrderCancellationReason(TestOrderCancellationReasonEnum.Other.Name);

    protected TestOrderCancellationReason() { } // EF Core
}

public abstract class TestOrderCancellationReasonEnum(string name, int value)
    : SmartEnum<TestOrderCancellationReasonEnum>(name, value)
{
    public static readonly TestOrderCancellationReasonEnum Qns = new QnsType();
    public static readonly TestOrderCancellationReasonEnum Abandoned = new AbandonedType();
    public static readonly TestOrderCancellationReasonEnum Other = new OtherType();
    
    private class QnsType() : TestOrderCancellationReasonEnum("QNS", 0);
    private class AbandonedType() : TestOrderCancellationReasonEnum("Abandoned", 1);
    private class OtherType() : TestOrderCancellationReasonEnum("Other", 100);
}