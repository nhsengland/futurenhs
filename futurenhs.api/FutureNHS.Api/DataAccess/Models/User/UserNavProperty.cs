namespace FutureNHS.Api.DataAccess.Models.User
{
    public record UserNavProperty
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public ImageData? Image { get; init; }
    }
}
