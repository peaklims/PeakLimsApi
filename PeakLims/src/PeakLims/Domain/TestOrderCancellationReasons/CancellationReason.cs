namespace PeakLims.Domain.TestOrderCancellationReasons;

using Ardalis.SmartEnum;
using Exceptions;

public class CancellationReason : ValueObject
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
    
    public CancellationReason(string value)
    {
        Value = value;
    }
    public CancellationReason(TestOrderCancellationReasonEnum value)
    {
        Value = value.Name;
    }
    
    public static CancellationReason Of(string value) => new CancellationReason(value);
    public static implicit operator string(CancellationReason value) => value == null ? null : value.Value;
    public static List<string> ListNames() => TestOrderCancellationReasonEnum.List.Select(x => x.Name).ToList();

    public static CancellationReason Qns() => new CancellationReason(TestOrderCancellationReasonEnum.Qns.Name);
    public static CancellationReason Abandoned() => new CancellationReason(TestOrderCancellationReasonEnum.Abandoned.Name);
    public static CancellationReason Other() => new CancellationReason(TestOrderCancellationReasonEnum.Other.Name);

    protected CancellationReason() { } // EF Core
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