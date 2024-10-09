namespace PeakLims.Services;

using Databases;
using Domain.RolePermissions;
using Domain.Roles;
using Domain.Users.Dtos;
using Domain.Users.Features;
using Domain.Users;
using Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UserPolicyHandler(
    ICurrentUserService currentUserService,
    PeakLimsDbContext dbContext,
    IMediator mediator)
    : IUserPolicyHandler
{

    public async Task<IEnumerable<string>> GetUserPermissions()
    {
        var roles = await GetRoles();

        // super admins can do everything
        if(roles.Contains(Role.SuperAdmin().Value))
            return Permissions.List();
        
        var permissions = await dbContext.RolePermissions
            .Where(x => roles.Contains(x.Role))
            .Select(x => x.Permission)
            .Distinct()
            .ToArrayAsync();

        return await Task.FromResult(permissions);
    }
    
    public async Task<bool> HasPermission(string permission)
    {
        var roles = await GetRoles();
    
        // super admins can do everything
        if (roles.Contains(Role.SuperAdmin().Value))
            return true;
        
        return await dbContext.RolePermissions
            .Where(rp => roles.Contains(rp.Role))
            .Select(rp => rp.Permission)
            .AnyAsync(x => x == permission);
    }

    private async Task<string[]> GetRoles()
    {
        var claimsPrincipal = currentUserService.User;
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        
        var nameIdentifier = currentUserService.UserIdentifier;
        var usersExist = dbContext.Users.Any();
        
        if (!usersExist)
            await SeedRootUser(nameIdentifier);

        var roles = !string.IsNullOrEmpty(nameIdentifier) 
            ? dbContext.UserRoles
                .Include(x => x.User)
                .Where(x => x.User.Identifier == nameIdentifier)
                .Select(x => x.Role.Value)
                .ToArray()
            : [];

        if (roles.Length == 0)
            throw new NoRolesAssignedException();

        return roles;
    }

    private async Task SeedRootUser(string userId)
    {
        var rootUser = new UserForCreationDto()
        {
            Username = currentUserService.Username,
            Email = currentUserService.Email,
            FirstName = currentUserService.FirstName,
            LastName = currentUserService.LastName,
            Identifier = userId
        };

        var userCommand = new AddUser.Command(rootUser, true);
        var createdUser = await mediator.Send(userCommand);

        var roleCommand = new AddUserRole.Command(createdUser.Id, Role.SuperAdmin().Value, true);
        await mediator.Send(roleCommand);
        
    }
}