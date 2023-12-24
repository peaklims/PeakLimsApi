namespace PeakLims.UnitTests.ServiceTests;

using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using MediatR;
using MockQueryable.NSubstitute;
using NSubstitute;
using PeakLims.Domain;
using PeakLims.Domain.RolePermissions;
using PeakLims.Domain.RolePermissions.Models;
using PeakLims.Domain.RolePermissions.Services;
using PeakLims.Domain.Roles;
using PeakLims.Domain.Users;
using PeakLims.Domain.Users.Services;
using PeakLims.Services;
using PeakLims.SharedTestHelpers.Fakes.User;

public class UserPolicyHandlerTests
{
    private readonly Faker _faker;

    public UserPolicyHandlerTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void GetUserPermissions_should_require_user()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        // Act
        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.User
            .Returns(claimsPrincipal);
        var rolePermissionsRepo = Substitute.For<IRolePermissionRepository>();
        var userRepo = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        var userPolicyHandler = new UserPolicyHandler(rolePermissionsRepo, currentUserService, userRepo, mediator);
        
        Func<Task> permissions = () => userPolicyHandler.GetUserPermissions();
        
        // Assert
        permissions.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task superadmin_user_gets_all_permissions()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.UsersExist();
        userRepo.SetRole(Role.SuperAdmin().Value);
        
        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.SetCurrentUser();
        var rolePermissionsRepo = Substitute.For<IRolePermissionRepository>();

        // Act
        var userPolicyHandler = new UserPolicyHandler(rolePermissionsRepo, currentUserService, userRepo, mediator);
        var permissions = await userPolicyHandler.GetUserPermissions();
        
        // Assert
        permissions.Should().BeEquivalentTo(Permissions.List().ToArray());
    }
    
    [Fact]
    public async Task superadmin_machine_gets_all_permissions()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.UsersExist();
        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.SetMachine();
        var rolePermissionsRepo = Substitute.For<IRolePermissionRepository>();
        
        userRepo.SetRole(Role.SuperAdmin().Value);
    
        // Act
        var userPolicyHandler = new UserPolicyHandler(rolePermissionsRepo, currentUserService, userRepo, mediator);
        var permissions = await userPolicyHandler.GetUserPermissions();
        
        // Assert
        permissions.Should().BeEquivalentTo(Permissions.List().ToArray());
    }
    
    [Fact]
    public async Task non_super_admin_gets_assigned_permissions_only()
    {
        // Arrange
        var permissionToAssign = _faker.PickRandom(Permissions.List());
        var randomOtherPermission = _faker.PickRandom(Permissions.List().Where(p => p != permissionToAssign));
        var nonSuperAdminRole = _faker.PickRandom(Role.ListNames().Where(p => p != Role.SuperAdmin().Value));
        
        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.SetCurrentUser();
        
        var mediator = Substitute.For<IMediator>();
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.UsersExist();
        userRepo.SetRole(nonSuperAdminRole);
    
        var rolePermission = RolePermission.Create(new RolePermissionForCreation()
        {
            Role = nonSuperAdminRole,
            Permission = permissionToAssign
        });
        var rolePermissions = new List<RolePermission>() {rolePermission};
        var mockData = rolePermissions.AsQueryable().BuildMock();
        var rolePermissionsRepo = Substitute.For<IRolePermissionRepository>();
        rolePermissionsRepo.Query()
            .Returns(mockData);
        
        // Act
    
        var userPolicyHandler = new UserPolicyHandler(rolePermissionsRepo, currentUserService, userRepo, mediator);
        var permissions = await userPolicyHandler.GetUserPermissions();
        
        // Assert
        permissions.Should().Contain(permissionToAssign);
        permissions.Should().NotContain(randomOtherPermission);
    }
    
    [Fact]
    public async Task claims_role_duplicate_permissions_removed()
    {
        // Arrange
        var permissionToAssign = _faker.PickRandom(Permissions.List());
        var nonSuperAdminRole = _faker.PickRandom(Role.ListNames().Where(p => p != Role.SuperAdmin().Value));
        
        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.SetCurrentUser();
        
        var mediator = Substitute.For<IMediator>();
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.UsersExist();
        userRepo.SetRole(nonSuperAdminRole);
    
        var rolePermission = RolePermission.Create(new RolePermissionForCreation()
        {
            Role = nonSuperAdminRole,
            Permission = permissionToAssign
        });
        var rolePermissions = new List<RolePermission>() {rolePermission, rolePermission};
        var mockData = rolePermissions.AsQueryable().BuildMock();
        var rolePermissionsRepo = Substitute.For<IRolePermissionRepository>();
        rolePermissionsRepo.Query()
            .Returns(mockData);
        
        // Act
        var userPolicyHandler = new UserPolicyHandler(rolePermissionsRepo, currentUserService, userRepo, mediator);
        var permissions = await userPolicyHandler.GetUserPermissions();
        
        // Assert
        permissions.Count(p => p == permissionToAssign).Should().Be(1);
        permissions.Should().Contain(permissionToAssign);
    }
}

public static class UserExtensions
{
    public static void SetRole(this IUserRepository repo, string role)
    {
        repo.GetRolesByUserIdentifier(Arg.Any<string>())
            .Returns(new List<string> { role });
    }

    public static void UsersExist(this IUserRepository repo)
    {
        var user = new FakeUserBuilder().Build();
        var users = new List<User>() {user};
        var mockData = users.AsQueryable().BuildMock();
        
        repo.Query().Returns(mockData);
    }
}
public static class CurrentUserServiceExtensions
{
    public static void SetCurrentUser(this ICurrentUserService repo, string nameIdentifier = null)
    {
        var user = SetUserClaim(nameIdentifier);
        repo.User
            .Returns(user);
        repo.UserId
            .Returns(user?.FindFirstValue(ClaimTypes.NameIdentifier));
        repo.IsMachine
            .Returns(false);
    }
    
    public static void SetMachine(this ICurrentUserService repo, string nameIdentifier = null, string clientId = null)
    {
        var machine = SetMachineClaim(nameIdentifier, clientId);
        repo.User
            .Returns(machine);
        repo.UserId
            .Returns(machine?.FindFirstValue(ClaimTypes.NameIdentifier));
        repo.IsMachine
            .Returns(true);
    }
    
    private static ClaimsPrincipal SetUserClaim(string nameIdentifier = null)
    {
        nameIdentifier ??= Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier)
        };

        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }
    
    private static ClaimsPrincipal SetMachineClaim(string nameIdentifier = null, string clientId = null)
    {
        nameIdentifier ??= Guid.NewGuid().ToString();
        clientId ??= Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
            new Claim("clientId", clientId)
        };

        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }
}