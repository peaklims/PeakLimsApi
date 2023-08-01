namespace PeakLims.Services;

using Ardalis.Specification;
using Domain.RolePermissions;
using Domain.Roles;
using Domain.Users.Dtos;
using Domain.Users.Features;
using Domain.Users.Services;
using Domain.Users;
using PeakLims.Domain.RolePermissions.Services;
using PeakLims.Domain;
using SharedKernel.Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UserPolicyHandler : IUserPolicyHandler
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public UserPolicyHandler(IRolePermissionRepository rolePermissionRepository, ICurrentUserService currentUserService, IUserRepository userRepository, IMediator mediator)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _mediator = mediator;
    }
    
    public async Task<IEnumerable<string>> GetUserPermissions()
    {
        var roles = await GetRoles();

        // super admins can do everything
        if(roles.Contains(Role.SuperAdmin().Value))
            return Permissions.List();

        var spec = new PermissionsFromRolesSpec(roles);
        var permissionsFromRoles = await _rolePermissionRepository.ListAsync(spec);
        return permissionsFromRoles.Distinct();
    }
    
    public async Task<bool> HasPermission(string permission)
    {
        var roles = await GetRoles();
    
        // super admins can do everything
        if (roles.Contains(Role.SuperAdmin().Value))
            return true;
        
        var spec = new HasPermissionSpec(roles, permission);
        return await _rolePermissionRepository.AnyAsync(spec);
    }

    private async Task<string[]> GetRoles()
    {
        var claimsPrincipal = _currentUserService.User;
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        
        var nameIdentifier = _currentUserService.UserId;
        var usersExist = await _userRepository.AnyAsync(new AllUserIdsSpec());
        
        if (!usersExist)
            await SeedRootUser(nameIdentifier);

        var roles = !string.IsNullOrEmpty(nameIdentifier) 
            ? _userRepository.GetRolesByUserIdentifier(nameIdentifier).ToArray() 
            : Array.Empty<string>();

        if (roles.Length == 0)
            throw new NoRolesAssignedException();

        return roles;
    }

    private async Task SeedRootUser(string userId)
    {
        var rootUser = new UserForCreationDto()
        {
            Username = _currentUserService.Username,
            Email = _currentUserService.Email,
            FirstName = _currentUserService.FirstName,
            LastName = _currentUserService.LastName,
            Identifier = userId
        };

        var userCommand = new AddUser.Command(rootUser, true);
        var createdUser = await _mediator.Send(userCommand);

        var roleCommand = new AddUserRole.Command(createdUser.Id, Role.SuperAdmin().Value, true);
        await _mediator.Send(roleCommand);
        
    }
}

public sealed class PermissionsFromRolesSpec : Specification<RolePermission, string>
{
    public PermissionsFromRolesSpec(string[] roles)
    {
        Query.Select(rp => rp.Permission)
            .Where(rp => roles.Contains(rp.Role.Value))
            .AsNoTracking();
    }
}

public sealed class HasPermissionSpec : Specification<RolePermission, string>
{
    public HasPermissionSpec(string[] roles, string permission)
    {
        Query.Select(rp => rp.Permission)
            .Where(rp => roles.Contains(rp.Role.Value) && rp.Permission == permission)
            .AsNoTracking();
    }
}

public sealed class AllUserIdsSpec : Specification<User, Guid>
{
    public AllUserIdsSpec()
    {
        Query.Select(x => x.Id).AsNoTracking();
    }
}