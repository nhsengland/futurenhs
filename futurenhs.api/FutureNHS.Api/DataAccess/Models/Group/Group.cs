namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record Group
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Strapline { get; init; }
        public string Slug { get; init; }
        public bool IsPublic { get; init; }
        public ImageData Image { get; init; }
        public Guid? ThemeId { get; init; }
        public string MemberStatus { get; init; }
        public Guid OwnerId { get; init; }
        public string OwnerFirstName { get; init; }
        public string OwnerSurname { get; init; }
    }
}
