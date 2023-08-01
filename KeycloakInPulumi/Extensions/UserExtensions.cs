namespace KeycloakInPulumi.Extensions;

using Pulumi;
using Pulumi.Keycloak;

public static class UserExtensions
{
    public static UserRoles SetRoles(this User user, params Input<string>[] userRoles)
    {
        return new UserRoles($"user-roles-{user.Id}", new UserRolesArgs()
        {
            UserId = user.Id,
            RealmId = user.RealmId,
            RoleIds = userRoles
        });
    }
}