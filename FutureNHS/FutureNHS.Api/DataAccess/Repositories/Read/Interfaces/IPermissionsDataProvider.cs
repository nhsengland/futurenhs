namespace FutureNHS.Api.DataAccess.Repositories.Read.Interfaces
{
    public interface IPermissionsDataProvider
    {
        Task<List<string>> GetSitePermissionsForRole(string role);
        Task<List<string>> GetGroupPermissionsForSiteRole(string role);
        Task<List<string>> GetPermissionsForGroupRole(string role, Guid groupId);
        Task<List<string>> GetPermissionsForGroupRole(string role, string slug);
    }
}
