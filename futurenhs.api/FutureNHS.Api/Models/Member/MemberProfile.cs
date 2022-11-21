using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.Models.Member
{ 
    public sealed record MemberProfile : BaseData
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string FullName => $"{FirstName} {LastName}";
        public string Pronouns { get; init; }
        public string Email { get; init; }
        public ImageData Image { get; init; }
        public Guid RoleId { get; init; }
        public Guid? ImageId { get; init; }
    }
}
