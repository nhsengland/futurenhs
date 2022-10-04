﻿using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.Group
{
    public record AdminGroupSummary
    {
        public Guid Id { get; init; }
        public string NameText { get; init; }
        public string StrapLineText { get; init; }
        public int MemberCount { get; init; }
        public int DiscussionCount { get; init; }
        public string Slug { get; init; }
        public ImageData Image { get; init; }
        public Guid? ThemeId { get; init; }
        public UserNavProperty Owner { get; init; }
        public bool IsPublic { get; init; }
        
        public bool IsDeleted { get; init; }
    }
}
