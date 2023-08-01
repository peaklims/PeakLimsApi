namespace PeakLims.SharedTestHelpers.Fakes.User;

using PeakLims.Domain.Users;
using PeakLims.Domain.Users.Models;

public class FakeUserBuilder
{
    private UserForCreation _creationData = new FakeUserForCreation().Generate();

    public FakeUserBuilder WithModel(UserForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeUserBuilder WithIdentifier(string identifier)
    {
        _creationData.Identifier = identifier;
        return this;
    }
    
    public FakeUserBuilder WithFirstName(string firstName)
    {
        _creationData.FirstName = firstName;
        return this;
    }
    
    public FakeUserBuilder WithLastName(string lastName)
    {
        _creationData.LastName = lastName;
        return this;
    }
    
    public FakeUserBuilder WithEmail(string email)
    {
        _creationData.Email = email;
        return this;
    }
    
    public FakeUserBuilder WithUsername(string username)
    {
        _creationData.Username = username;
        return this;
    }
    
    public User Build()
    {
        var result = User.Create(_creationData);
        return result;
    }
}