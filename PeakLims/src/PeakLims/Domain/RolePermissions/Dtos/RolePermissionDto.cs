namespace PeakLims.Domain.RolePermissions.Dtos;

public sealed class RolePermissionDto
{
    public Guid Id { get; set; }
    public string Role { get; set; }
    public string Permission { get; set; }
}
