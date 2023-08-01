namespace PeakLims.Domain.Users;

using PeakLims.Domain.Users.DomainEvents;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Roles;

public class UserRole : BaseEntity
{
    public User User { get; private set; }
    public Role Role { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete
    

    public static UserRole Create(User user, Role role)
    {
        var newUserRole = new UserRole
        {
            User = user,
            Role = role
        };

        newUserRole.QueueDomainEvent(new UserRolesUpdated(){ UserId = user.Id });
        
        return newUserRole;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected UserRole() { } // For EF + Mocking
}