namespace FutureNHS.Api.DataAccess.Models.Permissions
{
    public record UserAndGroupRoles(IEnumerable<string>? UserRoles, IEnumerable<GroupUserRole>? GroupUserRoles);
}
