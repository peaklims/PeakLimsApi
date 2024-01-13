namespace PeakLims.Domain.AuditLogConcepts;

using Ardalis.SmartEnum;
using AuditLogActions;
using PeakLims.Exceptions;

public sealed class AuditLogConcept : ValueObject
{
    private AuditLogConceptEnum _auditLogConcept;
    public string Value
    {
        get => _auditLogConcept.Name;
        private set
        {
            if (!AuditLogConceptEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException($"Invalid Audit log concept. PLease use one of the following: {string.Join(", ", ListNames())}");

            _auditLogConcept = parsed;
        }
    }
    
    public AuditLogConcept(string value)
    {
        Value = value;
    }

    public static AuditLogConcept Of(string value) => new AuditLogConcept(value);
    public static implicit operator string(AuditLogConcept value) => value.Value;
    public static List<string> ListNames() => AuditLogConceptEnum.List.Select(x => x.Name).ToList();

   public static AuditLogConcept Patient() => new AuditLogConcept(AuditLogConceptEnum.Patient.Name);
   public static AuditLogConcept User() => new AuditLogConcept(AuditLogConceptEnum.User.Name);
   public static AuditLogConcept UserRole() => new AuditLogConcept(AuditLogConceptEnum.UserRole.Name);
   public static AuditLogConcept RolePermission() => new AuditLogConcept(AuditLogConceptEnum.RolePermission.Name);
   public bool CanPerformAction(AuditLogAction action) => _auditLogConcept.CanPerformAction(action);

    private AuditLogConcept() { } // EF Core

    private abstract class AuditLogConceptEnum : SmartEnum<AuditLogConceptEnum>
    {
      public static readonly AuditLogConceptEnum Patient = new PatientType();
      public static readonly AuditLogConceptEnum User = new UserType();
      public static readonly AuditLogConceptEnum UserRole = new UserRoleType();
      public static readonly AuditLogConceptEnum RolePermission = new RolePermissionType();

       protected AuditLogConceptEnum(string name, int value) : base(name, value)
       {
       }

       public abstract bool CanPerformAction(AuditLogAction action); 

       private class PatientType : AuditLogConceptEnum
        {
            public PatientType() : base("Patient", 0)
            {
            }
            
            public override bool CanPerformAction(AuditLogAction action) => action == AuditLogAction.Added() 
                                                                               || action == AuditLogAction.Updated()
                                                                               || action == AuditLogAction.Deleted();
        }

       private class UserType : AuditLogConceptEnum
        {
            public UserType() : base("User", 1)
            {
            }
            
            public override bool CanPerformAction(AuditLogAction action) => action == AuditLogAction.Added() 
                                                                               || action == AuditLogAction.Updated()
                                                                               || action == AuditLogAction.Deleted()
                                                                               || action == AuditLogAction.Login();
        }

       private class UserRoleType : AuditLogConceptEnum
        {
            public UserRoleType() : base("UserRole", 2)
            {
            }
            
            public override bool CanPerformAction(AuditLogAction action) => action == AuditLogAction.Added() 
                                                                               || action == AuditLogAction.Updated()
                                                                               || action == AuditLogAction.Deleted();
        }

       private class RolePermissionType : AuditLogConceptEnum
        {
            public RolePermissionType() : base("RolePermission", 3)
            {
            }
            
            public override bool CanPerformAction(AuditLogAction action) => action == AuditLogAction.Added() 
                                                                               || action == AuditLogAction.Updated()
                                                                               || action == AuditLogAction.Deleted();
        }
    }
}