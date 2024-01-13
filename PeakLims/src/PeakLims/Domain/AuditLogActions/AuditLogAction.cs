namespace PeakLims.Domain.AuditLogActions;

using Ardalis.SmartEnum;
using PeakLims.Exceptions;

public sealed class AuditLogAction : ValueObject
{
    private AuditLogActionEnum _auditLogAction;
    public string Value
    {
        get => _auditLogAction.Name;
        private set
        {
            if (!AuditLogActionEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException($"Invalid Audit log action. PLease use one of the following: {string.Join(", ", ListNames())}");

            _auditLogAction = parsed;
        }
    }
    
    public AuditLogAction(string value)
    {
        Value = value;
    }

    public static AuditLogAction Of(string value) => new AuditLogAction(value);
    public static implicit operator string(AuditLogAction value) => value.Value;
    public static List<string> ListNames() => AuditLogActionEnum.List.Select(x => x.Name).ToList();

   public static AuditLogAction Added() => new AuditLogAction(AuditLogActionEnum.Added.Name);
   public static AuditLogAction Updated() => new AuditLogAction(AuditLogActionEnum.Updated.Name);
   public static AuditLogAction Deleted() => new AuditLogAction(AuditLogActionEnum.Deleted.Name);
   public static AuditLogAction Login() => new AuditLogAction(AuditLogActionEnum.Login.Name);

    private AuditLogAction() { } // EF Core

    private abstract class AuditLogActionEnum : SmartEnum<AuditLogActionEnum>
    {
      public static readonly AuditLogActionEnum Added = new AddedType();
      public static readonly AuditLogActionEnum Updated = new UpdatedType();
      public static readonly AuditLogActionEnum Deleted = new DeletedType();
      public static readonly AuditLogActionEnum Login = new LoginType();

       protected AuditLogActionEnum(string name, int value) : base(name, value)
       {
       }

       private class AddedType : AuditLogActionEnum
        {
            public AddedType() : base("Added", 0)
            {
            }
        }

       private class UpdatedType : AuditLogActionEnum
        {
            public UpdatedType() : base("Updated", 1)
            {
            }
        }

       private class DeletedType : AuditLogActionEnum
        {
            public DeletedType() : base("Deleted", 2)
            {
            }
        }

       private class LoginType : AuditLogActionEnum
        {
            public LoginType() : base("Login", 3)
            {
            }
        }
    }
}