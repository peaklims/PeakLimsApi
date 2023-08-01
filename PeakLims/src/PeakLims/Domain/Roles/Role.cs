namespace PeakLims.Domain.Roles;

using SharedKernel.Exceptions;
using SharedKernel.Domain;
using Ardalis.SmartEnum;

public class Role : ValueObject
{
    private RoleEnum _role;
    public string Value
    {
        get => _role.Name;
        private set
        {
            if (!RoleEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _role = parsed;
        }
    }
    
    public Role(string value)
    {
        Value = value;
    }
    public Role(RoleEnum value)
    {
        Value = value.Name;
    }
    
    public static Role Of(string value) => new Role(value);
    public static implicit operator string(Role value) => value.Value;
    public static List<string> ListNames() => RoleEnum.List.Select(x => x.Name).ToList();

    public static Role User() => new Role(RoleEnum.User.Name);
    public static Role SuperAdmin() => new Role(RoleEnum.SuperAdmin.Name);

    protected Role() { } // EF Core
}

public abstract class RoleEnum : SmartEnum<RoleEnum>
{
    public static readonly RoleEnum User = new UserType();
    public static readonly RoleEnum SuperAdmin = new SuperAdminType();

    protected RoleEnum(string name, int value) : base(name, value)
    {
    }

    private class UserType : RoleEnum
    {
        public UserType() : base("User", 0)
        {
        }
    }

    private class SuperAdminType : RoleEnum
    {
        public SuperAdminType() : base("Super Admin", 1)
        {
        }
    }
}