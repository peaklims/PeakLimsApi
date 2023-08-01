namespace PeakLims.Domain.Users.Mappings;

using PeakLims.Domain.Users.Dtos;
using PeakLims.Domain.Users.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class UserMapper
{
    public static partial UserForCreation ToUserForCreation(this UserForCreationDto userForCreationDto);
    public static partial UserForUpdate ToUserForUpdate(this UserForUpdateDto userForUpdateDto);
    public static partial UserDto ToUserDto(this User user);
    public static partial IQueryable<UserDto> ToUserDtoQueryable(this IQueryable<User> user);
}