namespace PeakLims.Domain.SampleStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class SampleStatus : ValueObject
{
    private SampleStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!SampleStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public SampleStatus(string value)
    {
        Value = value;
    }
    
    public static SampleStatus Of(string value) => new SampleStatus(value);
    public static implicit operator string(SampleStatus value) => value.Value;
    public static List<string> ListNames() => SampleStatusEnum.List.Select(x => x.Name).ToList();

    public static SampleStatus Received() => new SampleStatus(SampleStatusEnum.Received.Name);
    public static SampleStatus Rejected() => new SampleStatus(SampleStatusEnum.Rejected.Name);
    public static SampleStatus Disposed() => new SampleStatus(SampleStatusEnum.Disposed.Name);

    protected SampleStatus() { } // EF Core

    private abstract class SampleStatusEnum : SmartEnum<SampleStatusEnum>
    {
        public static readonly SampleStatusEnum Received = new ReceivedType();
        public static readonly SampleStatusEnum Rejected = new RejectedType();
        public static readonly SampleStatusEnum Disposed = new DisposedType();

        protected SampleStatusEnum(string name, int value) : base(name, value)
        {
        }

        public abstract bool IsFinalState();

        private class ReceivedType : SampleStatusEnum
        {
            public ReceivedType() : base("Received", 0)
            {
            }

            public override bool IsFinalState() => false;
        }

        private class RejectedType : SampleStatusEnum
        {
            public RejectedType() : base("Rejected", 1)
            {
            }

            public override bool IsFinalState() => false;
        }

        private class DisposedType : SampleStatusEnum
        {
            public DisposedType() : base("Disposed", 2)
            {
            }

            public override bool IsFinalState() => false;
        }
    }
}