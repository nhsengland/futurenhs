namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record Group
    {
        public Guid Id { get; init; }
        public string Name{ get; init; }
        public string StrapLine { get; init; }
        public string Slug { get; init; }
        public bool IsPublic { get; init; }
        public ImageData Image { get; init; }
        public Guid? ThemeId { get; init; }
        public bool IsMember { get; init; }

    }
}
