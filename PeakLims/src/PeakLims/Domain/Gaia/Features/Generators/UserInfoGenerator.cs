namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using Databases;
using Models;
using Users;
using Users.Models;
using Serilog;
using Services.External.Keycloak;
using Services.External.Keycloak.Models;

public interface IUserInfoGenerator
{
    Task<List<UserForCreation>> Generate(Guid organizationId, string domain);
}

public class UserInfoGenerator(IKeycloakClient keycloakClient, PeakLimsDbContext dbContext) : IUserInfoGenerator
{
    public async Task<List<UserForCreation>> Generate(Guid organizationId, string domain)
    {
        var random = new Random();
        var userCount = random.Next(3, 8);
        var people = PersonInfoGenerator.Generate(userCount);
        var users = new ConcurrentBag<UserForCreation>();

        async ValueTask GenerateUsers(PersonInfo person, CancellationToken ct)
        {
            var user = await CreateUser(person, organizationId, domain);
            users.Add(user);
        }
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 100
        };
        await Parallel.ForEachAsync(people, options, GenerateUsers);

        return users.ToList();
    }

    private async Task<UserForCreation> CreateUser(PersonInfo personInfo, Guid organizationId, string domain)
    {
        var email = $"{personInfo.FirstName.ToLower()}.{personInfo.LastName.ToLower()}@{domain}";
        var userToCreate = new UserForCreation()
        {
            Identifier = Guid.NewGuid().ToString(),
            FirstName = personInfo.FirstName,
            LastName = personInfo.LastName,
            Email = email,
            Username = email
        };

        var kcUser = new UserRepresentation()
        {
            Id = userToCreate.Identifier,
            FirstName = userToCreate.FirstName,
            LastName = userToCreate.LastName,
            Email = userToCreate.Email,
            Username = userToCreate.Username,
            Enabled = true,
            EmailVerified = true,
            RequiredActions = [ "UPDATE_PASSWORD" ],
            Credentials =
            [
                new()
                {
                    Type = "password",
                    Temporary = true,
                    Value = $"{userToCreate.FirstName.ToLower()}"
                }
            ],
            Attributes = new Dictionary<string, ICollection<string>>()
            {
                { "organization-id", new List<string>() { organizationId.ToString() } }
            }
        };
            
        Log.Information("User to create: {@User}", userToCreate);
        
        await keycloakClient.CreateUserAsync(kcUser);
        var user = User.Create(userToCreate);
        await dbContext.Users.AddAsync(user);
        // TODO add role
        
        return userToCreate;
    }
}