namespace PeakLims.Domain.TestOrderStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class TestOrderStatus : ValueObject
{
    private TestOrderStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!TestOrderStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public TestOrderStatus(string value)
    {
        Value = value;
    }
    public TestOrderStatus(TestOrderStatusEnum value)
    {
        Value = value.Name;
    }
    
    public static TestOrderStatus Of(string value) => new TestOrderStatus(value);
    public static implicit operator string(TestOrderStatus value) => value.Value;
    public static List<string> ListNames() => TestOrderStatusEnum.List.Select(x => x.Name).ToList();

    public static TestOrderStatus Pending() => new TestOrderStatus(TestOrderStatusEnum.Pending.Name);
    public static TestOrderStatus ReadyForTesting() => new TestOrderStatus(TestOrderStatusEnum.ReadyForTesting.Name);
    public static TestOrderStatus Testing() => new TestOrderStatus(TestOrderStatusEnum.Testing.Name);
    public static TestOrderStatus TestingComplete() => new TestOrderStatus(TestOrderStatusEnum.TestingComplete.Name);
    public static TestOrderStatus ReportPending() => new TestOrderStatus(TestOrderStatusEnum.ReportPending.Name);
    public static TestOrderStatus ReportComplete() => new TestOrderStatus(TestOrderStatusEnum.ReportComplete.Name);
    public static TestOrderStatus Completed() => new TestOrderStatus(TestOrderStatusEnum.Completed.Name);
    public static TestOrderStatus Cancelled() => new TestOrderStatus(TestOrderStatusEnum.Cancelled.Name);
    public bool IsFinalState() => _status.IsFinalState();
    public bool IsProcessing() => Pending().Value != Value;

    protected TestOrderStatus() { } // EF Core
}

public abstract class TestOrderStatusEnum : SmartEnum<TestOrderStatusEnum>
{
    public static readonly TestOrderStatusEnum Pending = new PendingType();
    public static readonly TestOrderStatusEnum ReadyForTesting = new ReadyForTestingType();
    public static readonly TestOrderStatusEnum Testing = new TestingType();
    public static readonly TestOrderStatusEnum TestingComplete = new TestingCompleteType();
    public static readonly TestOrderStatusEnum ReportPending = new ReportPendingType();
    public static readonly TestOrderStatusEnum ReportComplete = new ReportCompleteType();
    public static readonly TestOrderStatusEnum Completed = new CompletedType();
    public static readonly TestOrderStatusEnum Abandoned = new AbandonedType();
    public static readonly TestOrderStatusEnum Cancelled = new CancelledType();
    public static readonly TestOrderStatusEnum Qns = new QnsType();

    protected TestOrderStatusEnum(string name, int value) : base(name, value)
    {
    }

    public abstract bool IsFinalState();

    private class PendingType : TestOrderStatusEnum
    {
        public PendingType() : base("Pending", 0)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class ReadyForTestingType : TestOrderStatusEnum
    {
        public ReadyForTestingType() : base("Ready For Testing", 1)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class TestingType : TestOrderStatusEnum
    {
        public TestingType() : base("Testing", 2)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class TestingCompleteType : TestOrderStatusEnum
    {
        public TestingCompleteType() : base("Testing Complete", 3)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class ReportPendingType : TestOrderStatusEnum
    {
        public ReportPendingType() : base("Report Pending", 4)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class ReportCompleteType : TestOrderStatusEnum
    {
        public ReportCompleteType() : base("Report Complete", 5)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class CompletedType : TestOrderStatusEnum
    {
        public CompletedType() : base("Completed", 6)
        {
        }

        public override bool IsFinalState() => true;
    }

    private class AbandonedType : TestOrderStatusEnum
    {
        public AbandonedType() : base("Abandoned", 7)
        {
        }

        public override bool IsFinalState() => true;
    }

    private class CancelledType : TestOrderStatusEnum
    {
        public CancelledType() : base("Cancelled", 8)
        {
        }

        public override bool IsFinalState() => true;
    }

    private class QnsType : TestOrderStatusEnum
    {
        public QnsType() : base("QNS", 9)
        {
        }

        public override bool IsFinalState() => true;
    }
}