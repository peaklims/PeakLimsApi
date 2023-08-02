namespace PeakLims.Domain.PanelStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class PanelStatus : ValueObject
{
    private PanelStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!PanelStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public PanelStatus(string value)
    {
        Value = value;
    }
    public PanelStatus(PanelStatusEnum value)
    {
        Value = value.Name;
    }

    public bool IsActive() => Value == Active().Value;
    public static PanelStatus Of(string value) => new PanelStatus(value);
    public static implicit operator string(PanelStatus value) => value.Value;
    public static List<string> ListNames() => PanelStatusEnum.List.Select(x => x.Name).ToList();

    public static PanelStatus Draft() => new PanelStatus(PanelStatusEnum.Draft.Name);
    public static PanelStatus Active() => new PanelStatus(PanelStatusEnum.Active.Name);
    public static PanelStatus Inactive() => new PanelStatus(PanelStatusEnum.Inactive.Name);

    protected PanelStatus() { } // EF Core
}

public abstract class PanelStatusEnum : SmartEnum<PanelStatusEnum>
{
    public static readonly PanelStatusEnum Draft = new DraftType();
    public static readonly PanelStatusEnum Active = new ActiveType();
    public static readonly PanelStatusEnum Inactive = new InactiveType();

    protected PanelStatusEnum(string name, int value) : base(name, value)
    {
    }

    private class DraftType : PanelStatusEnum
    {
        public DraftType() : base("Draft", 0)
        {
        }
    }

    private class ActiveType : PanelStatusEnum
    {
        public ActiveType() : base("Active", 1)
        {
        }
    }

    private class InactiveType : PanelStatusEnum
    {
        public InactiveType() : base("Inactive", 2)
        {
        }
    }
}