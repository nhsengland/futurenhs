namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record GroupSite
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid ContentRootId { get; set; }
    }
}
