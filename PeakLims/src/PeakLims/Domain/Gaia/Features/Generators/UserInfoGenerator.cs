namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using Bogus.DataSets;
using Databases;
using Models;
using Users;
using Users.Models;
using Serilog;
using Services.External.Keycloak;
using Services.External.Keycloak.Models;

public interface IUserInfoGenerator
{
    Task<List<User>> Generate(Guid organizationId, string domain);
}

public class UserInfoGenerator(IKeycloakClient keycloakClient, PeakLimsDbContext dbContext) : IUserInfoGenerator
{
    public async Task<List<User>> Generate(Guid organizationId, string domain)
    {
        var random = new Random();
        var userCount = random.Next(3, 8);
        var people = PersonInfoGenerator.Generate(userCount);
        var knownPerson = new PersonInfo()
        {
            FirstName = "Pao",
            LastName = "Doe",
            Sex = Name.Gender.Male
        };
        people.Add(knownPerson);
        var userForCreations = new ConcurrentBag<UserForCreation>();

        async ValueTask GenerateUsers(PersonInfo person, CancellationToken ct)
        {
            var user = await CreateUser(person, organizationId, domain);
            userForCreations.Add(user);
        }
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 100
        };
        await Parallel.ForEachAsync(people, options, GenerateUsers);

        var usersInfoToCreate = userForCreations.ToList();
        var users = new List<User>();
        foreach (var userForCreation in usersInfoToCreate)
        {
            var user = User.Create(userForCreation);
            users.Add(user);
            await dbContext.Users.AddAsync(user);
        }
        // TODO add role
        return users.ToList();
    }

    private async Task<UserForCreation> CreateUser(PersonInfo personInfo, Guid organizationId, string domain)
    {
        var email = $"{personInfo.FirstName.ToLower()}.{personInfo.LastName.ToLower()}@{domain}";
        var userToCreate = new UserForCreation()
        {
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
        await keycloakClient.CreateUserAsync(kcUser);
        var kcUserResponse = await keycloakClient.GetUserByEmail(email);
        
        userToCreate.Identifier = kcUserResponse.Id;
        Log.Information("User to create: {@User}", userToCreate);
        
        return userToCreate;
    }
}