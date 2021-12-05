
using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Infrastructure.Models.GroupPages
{
    public record GroupHeader
    {
        public GroupHeader()
        {
        }

        public Guid Id { get; init; }
        public string NameText { get; init; }
        public string StrapLineText { get; init; }
        public string Slug { get; init; }
        private bool? UserApproved { get; init; }
        public string UserStatus
        {
            get
            {
                switch (UserApproved)
                {
                    case null:
                        return "Non-member";
                    case true:
                        return "Approved-member";
                    default:
                        return "Pending-member";
                }
            } }
        public IEnumerable<UrlLink> UserGroupActions { get; set; }

        public ImageData Image { get; init; }
    }
}
