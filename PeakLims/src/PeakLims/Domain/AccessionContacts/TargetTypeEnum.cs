namespace PeakLims.Domain.AccessionContacts;

using Ardalis.SmartEnum;

public abstract class TargetTypeEnum : SmartEnum<TargetTypeEnum>
{
    public static readonly TargetTypeEnum Email = new EmailType();

    protected TargetTypeEnum(string name, int value) : base(name, value)
    {
    }

    private class EmailType : TargetTypeEnum
    {
        public EmailType() : base("Email", 0)
        {
        }
    }
}