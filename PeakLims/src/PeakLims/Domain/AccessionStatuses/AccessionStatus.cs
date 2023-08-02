namespace PeakLims.Domain.AccessionStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class AccessionStatus : ValueObject
{
    private AccessionStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!AccessionStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public AccessionStatus(string value)
    {
        Value = value;
    }
    public AccessionStatus(AccessionStatusEnum value)
    {
        Value = value.Name;
    }
    
    public static AccessionStatus Of(string value) => new AccessionStatus(value);
    public static implicit operator string(AccessionStatus value) => value.Value;
    public static List<string> ListNames() => AccessionStatusEnum.List.Select(x => x.Name).ToList();

    public static AccessionStatus Draft() => new AccessionStatus(AccessionStatusEnum.Draft.Name);
    public static AccessionStatus ReadyForTesting() => new AccessionStatus(AccessionStatusEnum.ReadyForTesting.Name);
    public static AccessionStatus Testing() => new AccessionStatus(AccessionStatusEnum.Testing.Name);
    public static AccessionStatus TestingComplete() => new AccessionStatus(AccessionStatusEnum.TestingComplete.Name);
    public static AccessionStatus ReportPending() => new AccessionStatus(AccessionStatusEnum.ReportPending.Name);
    public static AccessionStatus ReportComplete() => new AccessionStatus(AccessionStatusEnum.ReportComplete.Name);
    public static AccessionStatus Completed() => new AccessionStatus(AccessionStatusEnum.Completed.Name);
    public static AccessionStatus Abandoned() => new AccessionStatus(AccessionStatusEnum.Abandoned.Name);
    public static AccessionStatus Cancelled() => new AccessionStatus(AccessionStatusEnum.Cancelled.Name);
    public static AccessionStatus Qns() => new AccessionStatus(AccessionStatusEnum.Qns.Name);
    public bool IsFinalState() => _status.IsFinalState();
    public bool IsProcessing() => Draft().Value != Value;

    protected AccessionStatus() { } // EF Core
}

public abstract class AccessionStatusEnum : SmartEnum<AccessionStatusEnum>
{
    public static readonly AccessionStatusEnum Draft = new DraftType();
    public static readonly AccessionStatusEnum ReadyForTesting = new ReadyForTestingType();
    public static readonly AccessionStatusEnum Testing = new TestingType();
    public static readonly AccessionStatusEnum TestingComplete = new TestingCompleteType();
    public static readonly AccessionStatusEnum ReportPending = new ReportPendingType();
    public static readonly AccessionStatusEnum ReportComplete = new ReportCompleteType();
    public static readonly AccessionStatusEnum Completed = new CompletedType();
    public static readonly AccessionStatusEnum Abandoned = new AbandonedType();
    public static readonly AccessionStatusEnum Cancelled = new CancelledType();
    public static readonly AccessionStatusEnum Qns = new QnsType();

    protected AccessionStatusEnum(string name, int value) : base(name, value)
    {
    }

    public abstract bool IsFinalState();

    private class DraftType : AccessionStatusEnum
    {
        public DraftType() : base("Draft", 0)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class ReadyForTestingType : AccessionStatusEnum
    {
        public ReadyForTestingType() : base("Ready For Testing", 1)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class TestingType : AccessionStatusEnum
    {
        public TestingType() : base("Testing", 2)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class TestingCompleteType : AccessionStatusEnum
    {
        public TestingCompleteType() : base("Testing Complete", 3)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class ReportPendingType : AccessionStatusEnum
    {
        public ReportPendingType() : base("Report Pending", 4)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class ReportCompleteType : AccessionStatusEnum
    {
        public ReportCompleteType() : base("Report Complete", 5)
        {
        }

        public override bool IsFinalState() => false;
    }

    private class CompletedType : AccessionStatusEnum
    {
        public CompletedType() : base("Completed", 6)
        {
        }

        public override bool IsFinalState() => true;
    }

    private class AbandonedType : AccessionStatusEnum
    {
        public AbandonedType() : base("Abandoned", 7)
        {
        }

        public override bool IsFinalState() => true;
    }

    private class CancelledType : AccessionStatusEnum
    {
        public CancelledType() : base("Cancelled", 8)
        {
        }

        public override bool IsFinalState() => true;
    }

    private class QnsType : AccessionStatusEnum
    {
        public QnsType() : base("QNS", 9)
        {
        }

        public override bool IsFinalState() => true;
    }
}