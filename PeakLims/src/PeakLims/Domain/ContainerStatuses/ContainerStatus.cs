namespace PeakLims.Domain.ContainerStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class ContainerStatus : ValueObject
{
    private ContainerStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!ContainerStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public ContainerStatus(string value)
    {
        Value = value;
    }
    public ContainerStatus(ContainerStatusEnum value)
    {
        Value = value.Name;
    }

    public bool IsActive() => Value == Active().Value;
    public static ContainerStatus Of(string value) => new ContainerStatus(value);
    public static implicit operator string(ContainerStatus value) => value.Value;
    public static List<string> ListNames() => ContainerStatusEnum.List.Select(x => x.Name).ToList();
    public static ContainerStatus Active() => new ContainerStatus(ContainerStatusEnum.Active.Name);
    public static ContainerStatus Inactive() => new ContainerStatus(ContainerStatusEnum.Inactive.Name);

    protected ContainerStatus() { } // EF Core
}

public abstract class ContainerStatusEnum : SmartEnum<ContainerStatusEnum>
{
    public static readonly ContainerStatusEnum Active = new ActiveType();
    public static readonly ContainerStatusEnum Inactive = new InactiveType();

    protected ContainerStatusEnum(string name, int value) : base(name, value)
    {
    }

    private class ActiveType : ContainerStatusEnum
    {
        public ActiveType() : base("Active", 0)
        {
        }
    }

    private class InactiveType : ContainerStatusEnum
    {
        public InactiveType() : base("Inactive", 1)
        {
        }
    }
}