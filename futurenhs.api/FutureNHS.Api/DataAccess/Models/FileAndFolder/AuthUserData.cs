namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record AuthUserData
    {
        public Guid Id { get; init; }
        public string EmailAddress { get; init; }
        public string FullName { get; init; }
        public string Initials { get; init; }
        public Guid FileId { get; init; }
        public string GroupSlug { get; init; }
    }
}
