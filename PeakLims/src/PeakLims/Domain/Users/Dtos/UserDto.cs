namespace PeakLims.Domain.Users.Dtos;

public sealed class UserDto
{
    public Guid Id { get; set; }
    public string Identifier { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }

}
