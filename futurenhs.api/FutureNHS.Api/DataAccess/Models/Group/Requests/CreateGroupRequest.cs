namespace FutureNHS.Api.DataAccess.Models.Group.Requests
{
    public class CreateGroupRequest
    {
        public sealed class GroupCreateRequest
        {
            public string Name { get; init; }
            public string Strapline { get; init; }
            public Guid ImageId { get; init; }
            public Guid GroupOwner { get; init; }
            public Guid[] GroupAdministrators { get; init; }
        }
    }
}
