namespace FileServer.Models
{
    public record AuthUser
    {
        public Guid Id { get; init; }
        public string EmailAddress { get; init; }
        public string FullName { get; init; }
        public string Initials { get; init; }
    }
}
