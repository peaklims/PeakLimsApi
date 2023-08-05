namespace PeakLims.Domain.HealthcareOrganizationStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class HealthcareOrganizationStatus : ValueObject
{
    private HealthcareOrganizationStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!HealthcareOrganizationStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public HealthcareOrganizationStatus(string value)
    {
        Value = value;
    }
    public HealthcareOrganizationStatus(HealthcareOrganizationStatusEnum value)
    {
        Value = value.Name;
    }

    public bool IsActive() => Value == Active().Value;
    public static HealthcareOrganizationStatus Of(string value) => new HealthcareOrganizationStatus(value);
    public static implicit operator string(HealthcareOrganizationStatus value) => value.Value;
    public static List<string> ListNames() => HealthcareOrganizationStatusEnum.List.Select(x => x.Name).ToList();
    public static HealthcareOrganizationStatus Active() => new HealthcareOrganizationStatus(HealthcareOrganizationStatusEnum.Active.Name);
    public static HealthcareOrganizationStatus Inactive() => new HealthcareOrganizationStatus(HealthcareOrganizationStatusEnum.Inactive.Name);

    protected HealthcareOrganizationStatus() { } // EF Core
}

public abstract class HealthcareOrganizationStatusEnum : SmartEnum<HealthcareOrganizationStatusEnum>
{
    public static readonly HealthcareOrganizationStatusEnum Active = new ActiveType();
    public static readonly HealthcareOrganizationStatusEnum Inactive = new InactiveType();

    protected HealthcareOrganizationStatusEnum(string name, int value) : base(name, value)
    {
    }

    private class ActiveType : HealthcareOrganizationStatusEnum
    {
        public ActiveType() : base("Active", 1)
        {
        }
    }

    private class InactiveType : HealthcareOrganizationStatusEnum
    {
        public InactiveType() : base("Inactive", 2)
        {
        }
    }
}