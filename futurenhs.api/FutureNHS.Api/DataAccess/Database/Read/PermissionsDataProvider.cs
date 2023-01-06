using FutureNHS.Api.DataAccess.Database.Read.Interfaces;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class PermissionsDataProvider : IPermissionsDataProvider
    {
        private readonly ILogger<PermissionsDataProvider> _logger;
        private const string Schema = "https://schema.collaborate.future.nhs.uk";
        private const string DefaultApiVerison = "v1";

        public PermissionsDataProvider(ILogger<PermissionsDataProvider> logger)
        {
            _logger = logger;
        }

        public Task<List<string>> GetSitePermissionsForRole(string role)
        {
            var permissions = new List<string>();

            switch (role)
            {
                case "Admin":
                    permissions.Add($"{Schema}/admin/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/members/{DefaultApiVerison}/add");
                    permissions.Add($"{Schema}/members/{DefaultApiVerison}/edit");
                    permissions.Add($"{Schema}/members/{DefaultApiVerison}/delete");
                    permissions.Add($"{Schema}/members/{DefaultApiVerison}/list");
                    permissions.Add($"{Schema}/members/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/platform/{DefaultApiVerison}/search");
                    permissions.Add($"{Schema}/platform/{DefaultApiVerison}/invite");
                    permissions.Add($"{Schema}/domain/{DefaultApiVerison}/add");
                    permissions.Add($"{Schema}/domain/{DefaultApiVerison}/edit");
                    permissions.Add($"{Schema}/domain/{DefaultApiVerison}/delete");
                    permissions.Add($"{Schema}/domain/{DefaultApiVerison}/view");
                    break;
                case "Standard Members":
                    permissions.Add($"{Schema}/members/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/platform/{DefaultApiVerison}/search");
                    break;
                case "Guest":
                    break;
                default:
                    break;
            }

            return Task.FromResult(permissions);
        }

        public Task<List<string>> GetGroupPermissionsForSiteRole(string role)
        {
            var permissions = new List<string>();

            switch (role)
            {
                case "Admin":
                    permissions.Add($"{Schema}/members/{DefaultApiVerison}/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/download");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/invite");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/pending/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like/view");
                    permissions.Add($"{Schema}/platform/{DefaultApiVerison}/invite");
                    break;
                case "Standard Members":
                    break;
                case "Guest":
                    break;
                default:
                    break;
            }

            return Task.FromResult(permissions);
        }

        public Task<List<string>> GetPermissionsForGroupRole(string role, Guid groupId)
        {
            var permissions = new List<string>();

            switch (role)
            {
                case "Admin":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/download");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/invite");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/pending/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like");
                    break;
                case "Standard Members":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/leave");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/download");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like/view");
                    break;
                case "Guest":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/join");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like/view");
                    break;
                case "":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/join");
                    break;
                default:
                    break;
            }

            return Task.FromResult(permissions);
        }

        public Task<List<string>> GetPermissionsForGroupRole(string role, string slug)
        {
            var permissions = new List<string>();

            switch (role)
            {
                case "Admin":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/download");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/delete");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/pending/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like");
                    permissions.Add($"{Schema}/platform/{DefaultApiVerison}/invite");

                    break;
                case "Standard Members":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/leave");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/add");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/download");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/edit");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/like");
                    break;
                case "Guest":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/discussions/comments/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/folders/files/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/members/view");
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/join");
                    break;
                case "":
                    permissions.Add($"{Schema}/groups/{DefaultApiVerison}/join");
                    break;
                default:
                    break;
            }

            return Task.FromResult(permissions);
        }
    }
}
