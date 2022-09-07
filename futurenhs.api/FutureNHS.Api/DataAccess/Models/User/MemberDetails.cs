﻿namespace FutureNHS.Api.DataAccess.Models.User
{
    public record MemberDetails : BaseData
    {
        public Guid Id { get; init; }
        public string Slug { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Initials { get; init; }
        public string Email { get; init; }
        public string Pronouns { get; init; }
        public DateTime DateJoinedUtc { get; init; }
        public DateTime LastLoginUtc { get; init; }
        public Image ProfileImage { get; init; }
    }
}
