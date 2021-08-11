﻿namespace MvcForum.Web.ViewModels.Post
{
    using System.Collections.Generic;
    using Core.Models.Entities;
    using Core.Models.General;

    public class PostViewModel
    {
        public Post Post { get; set; }
        public Post ReplyingTo { get; set; }
        public string PermaLink { get; set; }
        public List<Vote> Votes { get; set; }
        public List<Favourite> Favourites { get; set; }
        public Topic ParentTopic { get; set; }
        public PermissionSet Permissions { get; set; }
        public bool AllowedToVote { get; set; }
        public bool AllowedToReply { get; set; }
        public bool HasVotedUp { get; set; }
        public bool HasVotedDown { get; set; }
        public int VoteCount { get; set; }
        public bool MemberHasFavourited { get; set; }
        public bool MemberIsOnline { get; set; }
        public bool ShowTopicName { get; set; }
        public bool MinimalPost { get; set; }
        public bool IsTrustedUser { get; set; }
        public PaginatedList<Post> Replies { get; set; }
        public PostViewModel LatestReply { get; set; }
        public bool IsFocusThread { get; set; }

        /// <summary>
        /// Gets or sets the page that the post is on.
        /// </summary>
        public int PageIndex { get; set; }
    }
}