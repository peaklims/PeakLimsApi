namespace PeakLims.Domain.PanelOrderStatuses;

using Ardalis.SmartEnum;
using Exceptions;

public class PanelOrderStatus : ValueObject
{
    private PanelOrderStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!PanelOrderStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public PanelOrderStatus(string value)
    {
        Value = value;
    }
    
    public static PanelOrderStatus Of(string value) => new PanelOrderStatus(value);
    public static implicit operator string(PanelOrderStatus value) => value.Value;
    public static List<string> ListNames() => PanelOrderStatusEnum.List.Select(x => x.Name).ToList();

    public static PanelOrderStatus Pending() => new PanelOrderStatus(PanelOrderStatusEnum.Pending.Name);
    public static PanelOrderStatus Processing() => new PanelOrderStatus(PanelOrderStatusEnum.Processing.Name);
    public static PanelOrderStatus Completed() => new PanelOrderStatus(PanelOrderStatusEnum.Completed.Name);
    public static PanelOrderStatus Cancelled() => new PanelOrderStatus(PanelOrderStatusEnum.Cancelled.Name);
    public static PanelOrderStatus Abandoned() => new PanelOrderStatus(PanelOrderStatusEnum.Abandoned.Name);
    public bool IsFinalState() => _status.IsFinalState();

    protected PanelOrderStatus() { } // EF Core
    
    private abstract class PanelOrderStatusEnum : SmartEnum<PanelOrderStatusEnum>
    {
        public static readonly PanelOrderStatusEnum Pending = new PendingType();
        public static readonly PanelOrderStatusEnum Processing = new ProcessingType();
        public static readonly PanelOrderStatusEnum Completed = new CompletedType();
        public static readonly PanelOrderStatusEnum Cancelled = new CancelledType();
        public static readonly PanelOrderStatusEnum Abandoned = new AbandonedType();

        protected PanelOrderStatusEnum(string name, int value) : base(name, value)
        {
        }

        public abstract bool IsFinalState();

        private class PendingType : PanelOrderStatusEnum
        {
            public PendingType() : base("Pending", 0)
            {
            }

            public override bool IsFinalState() => false;
        }

        private class ProcessingType : PanelOrderStatusEnum
        {
            public ProcessingType() : base("Processing", 1)
            {
            }

            public override bool IsFinalState() => false;
        }

        private class CompletedType : PanelOrderStatusEnum
        {
            public CompletedType() : base("Completed", 6)
            {
            }

            public override bool IsFinalState() => true;
        }

        private class AbandonedType : PanelOrderStatusEnum
        {
            public AbandonedType() : base("Abandoned", 7)
            {
            }

            public override bool IsFinalState() => true;
        }

        private class CancelledType : PanelOrderStatusEnum
        {
            public CancelledType() : base("Cancelled", 8)
            {
            }

            public override bool IsFinalState() => true;
        }
    }
}
